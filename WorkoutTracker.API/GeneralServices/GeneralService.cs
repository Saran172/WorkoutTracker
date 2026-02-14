using Microsoft.EntityFrameworkCore;
using Shared.Entities;
using Shared.PlannerDTO;
using Shared.WorkoutDTO;
using System.Data;
using CommonServices.DataAccess;
using WorkoutTracker.API.DbCon;
using WorkoutTracker.API.DecrptionServices;
using WorkoutTracker.API.EncryptionServices;

namespace WorkoutTracker.API.GeneralServices
{
    public class GeneralService : IGeneralService
    {
        private readonly WTContext _db;
        private readonly IConfiguration _configuration;
        private readonly IEnryptService _enryptService;
        private readonly ITokenDecryption _tokenDecryption;
        private readonly DataAcces _dataAcces;
        private string BaseConnectionstring;
        public GeneralService(WTContext wTContext, IConfiguration configuration, IEnryptService enryptService, ITokenDecryption tokenDecryption, DataAcces dataccess)
        {
            _db = wTContext;
            _configuration = configuration;
            _enryptService = enryptService;
            _tokenDecryption = tokenDecryption;
            _dataAcces = dataccess;
            SetConnectionstring();
        }
        private void SetConnectionstring()
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");
            BaseConnectionstring = connString;
        }

        public async Task<ResponseDto> GetAllUsers(HttpContext httpContext)
        {
            ResponseDto responseDto = new ResponseDto();

            try
            {
                var connection = BaseConnectionstring;
                DataSet DSOut = new DataSet();
                var decrptedtoken = _tokenDecryption.DecryptedToken(httpContext);
                int userId = Convert.ToInt32(decrptedtoken[0]);

                var user = await _db.Users.ToListAsync();

                if(user != null)
                {
                    responseDto.RespType = "Success";
                    responseDto.RespCode = "200";
                    responseDto.RespMessage = "Users retrieved successfully";
                    responseDto.RespData = user;
                }
                return responseDto;

            }
            catch (Exception ex)
            {
                responseDto.RespType = "Error";
                responseDto.RespCode = "500";
                responseDto.RespMessage = ex.Message.ToString();
                responseDto.RespData = null;

            }
            return responseDto;
        }
        public async Task<ResponseDto> GetUserInfo(HttpContext httpContext)
        {
            ResponseDto responseDto = new ResponseDto();

            try
            {
                var connection = BaseConnectionstring;
                DataSet DSOut = new DataSet();
                var decrptedtoken = _tokenDecryption.DecryptedToken(httpContext);
                int userId = Convert.ToInt32(decrptedtoken[1]);

                string QueryName = $"WT_GET_USER_INFO {userId}";

                if (connection != null)
                {
                    DSOut = new DataSet();
                    DSOut = _dataAcces.ExecuteDataSet(QueryName, connection);
                }
                if (DSOut.Tables.Count > 0)
                {
                    var Table1 = DSOut.Tables[0];
                    var table2 = DSOut.Tables[1];
                    responseDto.RespType = Table1.Rows[0]["respType"].ToString();
                    responseDto.RespCode = Table1.Rows[0]["respCode"].ToString();
                    responseDto.RespMessage = Table1.Rows[0]["respDesc"].ToString();

                    var responsedata = _dataAcces.DataTableToJSONWithJSONNet(table2);
                    responseDto.RespData = responsedata;
                    return responseDto;
                }
            }
            catch (Exception ex)
            {
                responseDto.RespType = "Error";
                responseDto.RespCode = "500";
                responseDto.RespMessage = ex.Message.ToString();
                responseDto.RespData = null;

            }
            return responseDto;
        }
        public async Task<ResponseDto> PostUserInfo(User User ,HttpContext httpContext)
        {
            ResponseDto responseDto = new ResponseDto();

            try
            {
                var connection = BaseConnectionstring;
                DataSet DSOut = new DataSet();
                DataSet DSIn = new DataSet("Root");
                DataTable dataTable = new DataTable();
                var decrptedtoken = _tokenDecryption.DecryptedToken(httpContext);
                int userId = Convert.ToInt32(decrptedtoken[1]);

                List<User> newl = new List<User>();
                newl.Add(User);
                dataTable = _dataAcces.ToDataTable(newl);
                dataTable.TableName = "Record";

                DSIn.Tables.Add(dataTable);
                var strXml = DSIn.GetXml();

                string QueryName = $"WT_POST_USER_INFO '{strXml}', {userId}";

                if (connection != null)
                {
                    DSOut = new DataSet();
                    DSOut = _dataAcces.ExecuteDataSet(QueryName, connection);
                }
                if (DSOut.Tables.Count > 0)
                {
                    var Table1 = DSOut.Tables[0];
                    var table2 = DSOut.Tables[1];
                    responseDto.RespType = Table1.Rows[0]["respType"].ToString();
                    responseDto.RespCode = Table1.Rows[0]["respCode"].ToString();
                    responseDto.RespMessage = Table1.Rows[0]["respDesc"].ToString();

                    var responsedata = _dataAcces.DataTableToJSONWithJSONNet(table2);
                    responseDto.RespData = responsedata;
                    return responseDto;
                }
            }
            catch (Exception ex)
            {
                responseDto.RespType = "Error";
                responseDto.RespCode = "500";
                responseDto.RespMessage = ex.Message.ToString();
                responseDto.RespData = null;

            }
            return responseDto;
        }





        public async Task<ResponseDto> CreateTemplate(TemplateUpsertDto dto, HttpContext httpContext)
        {
            var response = new ResponseDto();

            try
            {
                var decrptedtoken = _tokenDecryption.DecryptedToken(httpContext);
                int userId = Convert.ToInt32(decrptedtoken[0]);

                var template = new WorkoutTemplate
                {
                    UserId = userId,
                    Name = dto.Name.Trim(),
                    Type = dto.Type,
                    Description = dto.Description,
                    CreatedAt = DateTime.UtcNow
                };

                _db.WorkoutTemplates.Add(template);
                await _db.SaveChangesAsync();

                int order = 1;
                foreach (var exId in dto.ExerciseIds.Distinct())
                {
                    _db.WorkoutTemplateExercises.Add(new WorkoutTemplateExercise
                    {
                        TemplateId = template.Id,
                        ExerciseId = exId,
                        ExerciseOrder = order++
                    });
                }

                await _db.SaveChangesAsync();

                response.RespCode = "200";
                response.RespType = "Success";
                response.RespMessage = "Template created successfully";
                response.RespData = template.Id;
            }
            catch (Exception ex)
            {
                response.RespCode = "500";
                response.RespType = "Failure";
                response.RespMessage = ex.Message;
            }

            return response;
        }


        public async Task<ResponseDto> UpdateTemplate(long templateId, TemplateUpsertDto dto, HttpContext httpContext)
        {
            var response = new ResponseDto();

            try
            {
                var decrptedtoken = _tokenDecryption.DecryptedToken(httpContext);
                int userId = Convert.ToInt32(decrptedtoken[0]);

                var template = await _db.WorkoutTemplates
                    .FirstOrDefaultAsync(t =>
                        t.Id == templateId &&
                        t.UserId == userId &&
                        !t.IsDeleted);

                if (template == null)
                    throw new Exception("Template not found");

                template.Name = dto.Name.Trim();
                template.Type = dto.Type;
                template.Description = dto.Description;
                template.UpdatedAt = DateTime.UtcNow;

                // Soft delete old exercises
                await _db.WorkoutTemplateExercises
                    .Where(x => x.TemplateId == templateId && !x.IsDeleted)
                    .ExecuteUpdateAsync(u =>
                        u.SetProperty(p => p.IsDeleted, true));

                int order = 1;
                foreach (var exId in dto.ExerciseIds.Distinct())
                {
                    _db.WorkoutTemplateExercises.Add(new WorkoutTemplateExercise
                    {
                        TemplateId = templateId,
                        ExerciseId = exId,
                        ExerciseOrder = order++
                    });
                }

                await _db.SaveChangesAsync();

                response.RespCode = "200";
                response.RespType = "Success";
                response.RespMessage = "Template updated successfully";
            }
            catch (Exception ex)
            {
                response.RespCode = "500";
                response.RespType = "Failure";
                response.RespMessage = ex.Message;
            }

            return response;
        }


        public async Task<ResponseDto> DeleteTemplate(long templateId, HttpContext httpContext)
        {
            var response = new ResponseDto();

            try
            {
                var decrptedtoken = _tokenDecryption.DecryptedToken(httpContext);
                int userId = Convert.ToInt32(decrptedtoken[0]);

                var template = await _db.WorkoutTemplates
                    .FirstOrDefaultAsync(t =>
                        t.Id == templateId &&
                        t.UserId == userId &&
                        !t.IsDeleted);

                if (template == null)
                    throw new Exception("Template not found");

                template.IsDeleted = true;
                template.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                response.RespCode = "200";
                response.RespType = "Success";
                response.RespMessage = "Template deleted successfully";
            }
            catch (Exception ex)
            {
                response.RespCode = "500";
                response.RespType = "Failure";
                response.RespMessage = ex.Message;
            }

            return response;
        }


        public async Task<ResponseDto> GetTemplates(HttpContext httpContext)
        {
            var response = new ResponseDto();

            try
            {
                var decrptedtoken = _tokenDecryption.DecryptedToken(httpContext);
                int userId = Convert.ToInt32(decrptedtoken[0]);

                var templates = await _db.WorkoutTemplates
                    .Where(t => t.UserId == userId && !t.IsDeleted)
                    .Select(t => new TemplateListDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Type = t.Type,
                        ExerciseCount = _db.WorkoutTemplateExercises
                            .Count(e => e.TemplateId == t.Id && !e.IsDeleted)
                    })
                    .OrderByDescending(t => t.Id)
                    .ToListAsync();

                response.RespCode = "200";
                response.RespType = "Success";
                response.RespMessage = "Templates retrieved";
                response.RespData = templates;
            }
            catch (Exception ex)
            {
                response.RespCode = "500";
                response.RespType = "Failure";
                response.RespMessage = ex.Message;
            }

            return response;
        }
        
        public async Task<ResponseDto> GetTemplateDetail(long templateId, HttpContext httpContext)
        {
            var response = new ResponseDto();

            try
            {
                var decrptedtoken = _tokenDecryption.DecryptedToken(httpContext);
                int userId = Convert.ToInt32(decrptedtoken[0]);

                var template = await _db.WorkoutTemplates
                    .FirstOrDefaultAsync(t =>
                        t.Id == templateId &&
                        t.UserId == userId &&
                        !t.IsDeleted);

                if (template == null)
                    throw new Exception("Template not found");

                var exercises = await _db.WorkoutTemplateExercises
                    .Where(e => e.TemplateId == templateId && !e.IsDeleted)
                    .OrderBy(e => e.ExerciseOrder)
                    .Select(e => e.ExerciseId)
                    .ToListAsync();

                response.RespCode = "200";
                response.RespType = "Success";
                response.RespMessage = "Template details retrieved";
                response.RespData = new TemplateDetailDto
                {
                    Id = template.Id,
                    Name = template.Name,
                    Type = template.Type,
                    Description = template.Description,
                    ExerciseIds = exercises
                };
            }
            catch (Exception ex)
            {
                response.RespCode = "500";
                response.RespType = "Failure";
                response.RespMessage = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> GetExercises(HttpContext httpContext)
        {
            var response = new ResponseDto();

            try
            {
                var decrptedtoken = _tokenDecryption.DecryptedToken(httpContext);
                int userId = Convert.ToInt32(decrptedtoken[0]);

                var exercises = await _db.Exercises.OrderBy(x => x.Id).ToListAsync();

                response.RespCode = "200";
                response.RespType = "Success";
                response.RespMessage = "Exercises retrieved";
                response.RespData = exercises;
            }
            catch (Exception ex)
            {
                response.RespCode = "500";
                response.RespType = "Failure";
                response.RespMessage = ex.Message;
            }

            return response;
        }


        public async Task<ResponseDto> AddWorkoutSession(WorkoutSessionCreateDto dto, HttpContext httpContext)
        {
            var response = new ResponseDto();

            try
            {
                var decrptedtoken = _tokenDecryption.DecryptedToken(httpContext);
                int userId = Convert.ToInt32(decrptedtoken[0]);

                var session = new WorkoutSession
                {
                    UserId = userId,
                    WorkoutDate = DateTime.UtcNow.Date,
                    StartTime = dto.StartDateTime,
                    EndTime = dto.EndDateTime,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                _db.WorkoutSessions.Add(session);
                await _db.SaveChangesAsync();

                foreach (var ex in dto.Exercises)
                {
                    var sessionExercise = new WorkoutSessionExercise
                    {
                        WorkoutSessionId = session.Id,
                        ExerciseId = ex.ExerciseId
                    };

                    _db.WorkoutSessionExercises.Add(sessionExercise);
                    await _db.SaveChangesAsync();

                    foreach (var set in ex.Sets)
                    {
                        _db.WorkoutSessionSets.Add(new WorkoutSessionSet
                        {
                            WorkoutExerciseId = sessionExercise.Id,
                            SetNumber = set.SetNumber,
                            Weight = set.Weight,
                            Reps = set.Reps,
                            IsFailure = set.IsFailure
                        });
                    }
                }

                await _db.SaveChangesAsync();

                response.RespCode = "200";
                response.RespType = "Success";
                response.RespMessage = "Workout session saved";
                response.RespData = session.Id;
            }
            catch (Exception ex)
            {
                response.RespCode = "500";
                response.RespType = "Failure";
                response.RespMessage = ex.Message;
            }

            return response;
        }


    }
}
