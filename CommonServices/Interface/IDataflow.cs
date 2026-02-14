using Shared.Entities;
using Shared.PlannerDTO;
using Shared.WorkoutDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonServices.Interface
{
    public interface IDataflow
    {
        Task<ResponseDto> LoginAuth(string Email, string Password);
        Task<ResponseDto> Register(string fullname, string Email, string Password);
        Task<ResponseDto> AddTemplate(TemplateUpsertDto template, string JWT);
        Task<ResponseDto> GetTemplates(string JWT);
        Task<ResponseDto> GetTemplateDetail(long Id, string JWT);
        Task<ResponseDto> DeleteTemplate(long templateId, string JWT);
        Task<ResponseDto> GetExercises(string JWT);
        Task<ResponseDto> AddWorkoutSession(WorkoutSessionCreateDto workout, string JWT);
    }
}
