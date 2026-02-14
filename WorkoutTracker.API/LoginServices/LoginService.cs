using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shared.Entities;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkoutTracker.API.DbCon;
using WorkoutTracker.API.EncryptionServices;
using WorkoutTracker.API.GeneralServices;

namespace WorkoutTracker.API.LoginServices
{
    public class LoginService : IloginService
    {
        private readonly WTContext _db;
        private readonly IConfiguration _configuration;
        private readonly IEnryptService _enryptService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LoginService(WTContext wTContext, IConfiguration configuration, IEnryptService enrptService, IWebHostEnvironment webHostEnvironment)
        {
            _db = wTContext;
            _configuration = configuration;
            _enryptService = enrptService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ResponseDto> WebLogin(WebLoginDto LoginDto)
        {
            ResponseDto responseDto = new ResponseDto();
            try
            {
                var result = await AuthenticateUserAsync(LoginDto);
                if (result.UserCode != null)
                {
                    var Token = GenerateToken(result);
                    responseDto.RespCode = "200";
                    responseDto.RespMessage = "Token Generated Successfully";
                    responseDto.RespType = "Success";
                    responseDto.RespData = Token;
                }
            }
            catch (Exception ex)
            {
                responseDto.RespCode = "400";
                responseDto.RespMessage = ex.Message.ToString();
                responseDto.RespType = "Failure";
            }
            return responseDto;
        }
        public async Task<ResponseDto> WebRegister(WebRegisterDto registerDto)
        {
            var response = new ResponseDto();

            try
            {
                if (string.IsNullOrWhiteSpace(registerDto.UserName) ||
                    string.IsNullOrWhiteSpace(registerDto.Email) ||
                    string.IsNullOrWhiteSpace(registerDto.Password))
                {
                    return new ResponseDto
                    {
                        RespCode = "400",
                        RespType = "Failure",
                        RespMessage = "Username, Email and Password are required"
                    };
                }

                var email = registerDto.Email.Trim().ToLower();
                var userName = registerDto.UserName.Trim();

                var exists = await _db.Users.AnyAsync(u =>
                    u.Email == email);

                if (exists)
                {
                    return new ResponseDto
                    {
                        RespCode = "409",
                        RespType = "Failure",
                        RespMessage = "User already exists"
                    };
                }

                var user = new User
                {
                    UserCode = $"WT_{Guid.NewGuid():N}",
                    UserName = userName,
                    Email = email,
                    PasswordHash = PasswordHasher.Hash(registerDto.Password),
                    Status = 1,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                response.RespCode = "200";
                response.RespType = "Success";
                response.RespMessage = "User registered successfully";
                response.RespData = new
                {
                    user.Id,
                    user.UserCode,
                    user.UserName,
                    user.Email
                };
            }
            catch (DbUpdateException)
            {
                response.RespCode = "409";
                response.RespType = "Failure";
                response.RespMessage = "Email already exists";
            }
            catch (Exception)
            {
                response.RespCode = "500";
                response.RespType = "Failure";
                response.RespMessage = "Something went wrong";
            }

            return response;
        }



        public async Task<ResponseDto> MobileLogin(MobileLoginDto dto)
        {
            var response = new ResponseDto();

            try
            {
                var user = await AuthenticateUserAsync(dto);

                await UpsertDeviceAndTokenAsync(
                    user.Id,
                    dto.DeviceCode,
                    dto.FcmToken,
                    "android"
                );

                var token = GenerateToken(user);

                response.RespCode = "200";
                response.RespType = "Success";
                response.RespMessage = "Login successful";
                response.RespData = token;
            }
            catch (Exception ex)
            {
                response.RespCode = "401";
                response.RespType = "Failure";
                response.RespMessage = ex.Message;
            }

            return response;
        }



        private async Task<User> AuthenticateUserAsync(LoginBaseDto login)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == login.Email!.ToLower());

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!PasswordHasher.Verify(login.Password!, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            if (user.Status == 0)
                throw new UnauthorizedAccessException("Account is inactive");

            return user;
        }

        private async Task UpsertDeviceAndTokenAsync(long userId, string deviceCode, string? fcmToken, string platform)
        {
            var device = await _db.UserDevices
                .FirstOrDefaultAsync(d => d.UserId == userId && d.DeviceCode == deviceCode);

            if (device == null)
            {
                device = new UserDevice
                {
                    UserId = userId,
                    DeviceCode = deviceCode,
                    Platform = platform,
                    IsActive = true,
                    LastSeenAt = DateTime.UtcNow
                };

                _db.UserDevices.Add(device);
                await _db.SaveChangesAsync(); // need Id
            }
            else
            {
                device.LastSeenAt = DateTime.UtcNow;
            }

            if (!string.IsNullOrWhiteSpace(fcmToken))
            {
                // deactivate old tokens
                await _db.DevicePushTokens
                    .Where(t => t.DeviceId == device.Id && t.IsActive)
                    .ExecuteUpdateAsync(t => t.SetProperty(x => x.IsActive, false));

                _db.DevicePushTokens.Add(new DevicePushToken
                {
                    DeviceId = device.Id,
                    FcmToken = fcmToken,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync();
        }

        private string GenerateToken(User userMaster)
        {
            var Tokenkey = Encoding.UTF8.GetBytes(_configuration.GetRequiredSection("JwtSettings:SecurityKey").Value);
            var tokenhandler = new JwtSecurityTokenHandler();
            var claim = new Claim[]
            {
                  new Claim(ClaimTypes.NameIdentifier,userMaster.Id.ToString()),
                  new Claim("Id",userMaster.Id.ToString()),
                  new Claim("UserCode",userMaster.UserCode),
                  new Claim(ClaimTypes.Email, userMaster.Email)
            };
            var encrptedclaim = EncryptClaims(claim);
            var tokendescripter = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
            {
                new Claim("encryptedClaims", encrptedclaim)
            }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Tokenkey), SecurityAlgorithms.HmacSha512)
            };

            var token = tokenhandler.CreateToken(tokendescripter);

            var console = DateTime.UtcNow;
            Console.WriteLine(console);
            string finaltoken = tokenhandler.WriteToken(token);
            return finaltoken;
        }

        private string EncryptClaims(IEnumerable<Claim> claims)
        {
            var jsonClaims = new JwtPayload(claims);
            var jsonClaimsString = JsonConvert.SerializeObject(jsonClaims);
            return _enryptService.Encryption(jsonClaimsString);
        }
        public async Task<string> Decrption(string Claims, HttpContext httpContext)
        {
            var claimPrincipal = httpContext.User as ClaimsPrincipal;
            var encclaim = claimPrincipal.FindFirst("encryptedClaims")?.Value;
            return _enryptService.Decryption(Claims);
        }
    }
}
