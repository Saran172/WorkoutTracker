using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Shared.Entities;
using Shared.PlannerDTO;
using Shared.UiModels;

namespace WorkoutTracker.UI.Components.MainScreens
{
    public partial class Workout
    {
        public string JWT { get; set; }
        private List<TemplateListDto> savedTemplates = new();

        protected override async Task OnInitializedAsync()
        {
            var token = await SessionStorage.GetAsync<SessionDTO>("Session");
            JWT = token.Value.JWT;
            var response = await Flow.GetTemplates(JWT);
            if (response.RespData != null)
            {
                savedTemplates = JsonConvert.DeserializeObject<List<TemplateListDto>>(response.RespData.ToString());
            }
            StateHasChanged();
        }
        private void StartSession()
        {
            NavService.SetActiveMenu("ActiveWorkout");
            StateHasChanged();
        }
        private void NewTemplate_Click()
        {
            NavService.SetActiveMenu("Planner");
            StateHasChanged();
        }

        public async Task StartTemplate(long templateId)
        {
            var response = await Flow.GetTemplateDetail(templateId, JWT);
            if (response.RespCode == "200" && response.RespData != null)
            {
                WorkoutState.PendingTemplate = JsonConvert.DeserializeObject<TemplateDetailDto>(response.RespData.ToString());

                NavService.SetActiveMenu("ActiveWorkout"); 
            }
        }
    }
}
