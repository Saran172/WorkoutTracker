using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shared.UiModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CommonServices.DataAccess;

namespace WorkoutTracker.UI.Services.AuthenticationStateProvide
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ProtectedLocalStorage _localStorage;
        private const string SessionKey = "Session";

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage, ProtectedLocalStorage localStorage)
        {
            _sessionStorage = sessionStorage;
            _localStorage = localStorage;
        }
       
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _sessionStorage.GetAsync<SessionDTO>(SessionKey);

            if (!token.Success || token.Value == null || string.IsNullOrWhiteSpace(token.Value.JWT))
            {
                return Anonymous();
            }

            var identity = string.IsNullOrWhiteSpace(token.Value.JWT)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(ParseClaimsFromJwt(token.Value.JWT), "jwtAuthType");

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        private static AuthenticationState Anonymous() =>
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));


        public async Task Logout()
        {
            await _localStorage.DeleteAsync(SessionKey);
            await _sessionStorage.DeleteAsync(SessionKey);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }


        public async void NotifyUserAuthentication(string token)
        {
            DataAcces dataAcces = new DataAcces();
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt = tokenHandler.ReadJwtToken(token);

            var EncrptedClaim = jwt.Claims.FirstOrDefault(c => c.Type == "encryptedClaims")?.Value;
            var decryptedclaim = dataAcces.Decrypturls(EncrptedClaim);
            var decrptjson = JsonConvert.DeserializeObject<Dictionary<string, string>>(decryptedclaim);
            EncrptedClaim = jwt.Claims.FirstOrDefault(c => c.Type == "encryptedClaims")?.Value;
            string UsName = decrptjson["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
            string UserCode = decrptjson["UserCode"];

            SessionDTO Sesiondata = new SessionDTO();
            Sesiondata.Id = Convert.ToInt32(decrptjson["Id"]);
            Sesiondata.UserCode = decrptjson["UserCode"];
            Sesiondata.JWT = token;
            Sesiondata.UserName = UsName;
            await _sessionStorage.SetAsync(SessionKey, Sesiondata);


            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwtAuthType");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async void NotifyUserLogout()
        {
            await _sessionStorage.DeleteAsync("Session");
            await _localStorage.DeleteAsync("Session");

            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }
    }
}
