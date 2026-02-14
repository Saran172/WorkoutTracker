using Shared.Entities;
using Shared.PlannerDTO;
using Shared.WorkoutDTO;

namespace WorkoutTracker.API.GeneralServices
{
    public interface IGeneralService
    {
        Task<ResponseDto> GetAllUsers(HttpContext httpContext);
        Task<ResponseDto> GetUserInfo(HttpContext httpContext);
        Task<ResponseDto> PostUserInfo(User userMaster, HttpContext httpContext);
        Task<ResponseDto> CreateTemplate(TemplateUpsertDto dto, HttpContext httpContext);
        Task<ResponseDto> UpdateTemplate(long templateId, TemplateUpsertDto dto, HttpContext httpContext);
        Task<ResponseDto> DeleteTemplate(long templateId, HttpContext httpContext);
        Task<ResponseDto> GetTemplates(HttpContext httpContext);
        Task<ResponseDto> GetTemplateDetail(long templateId, HttpContext httpContext);
        Task<ResponseDto> GetExercises(HttpContext httpContext);
        Task<ResponseDto> AddWorkoutSession(WorkoutSessionCreateDto dto, HttpContext httpContext);

    }
}
