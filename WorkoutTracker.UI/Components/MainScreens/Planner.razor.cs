using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Entities;
using Shared.PlannerDTO;
using Shared.UiModels;
using System.Linq;
using System.Xml.Linq;

namespace WorkoutTracker.UI.Components.MainScreens
{
    public partial class Planner
    {
        public string JWT { get; set; }
        private bool isCreating = false;

        private TemplateUpsertDto newTemplate = new();

        private List<Exercise> AvailableExercises = new();
        private List<Exercise> selectedExercises = new();

        private List<TemplateListDto> savedTemplates = new();

       
        protected override async Task OnInitializedAsync()
        {
            var token = await SessionStorage.GetAsync<SessionDTO>("Session");
            JWT = token.Value.JWT;

            var exerciseResponse = await Flow.GetExercises(JWT);
            if(exerciseResponse.RespData != null)
            {
                AvailableExercises = JsonConvert.DeserializeObject<List<Exercise>>(exerciseResponse.RespData.ToString());
            }

            var response = await Flow.GetTemplates(JWT);
            if (response.RespData != null)
            {
                savedTemplates = JsonConvert.DeserializeObject<List<TemplateListDto>>(response.RespData.ToString());
            }
        }
        



        public void ToggleCreate()
        {
            isCreating = !isCreating;
        }

        private void AddExercise(ChangeEventArgs e)
        {
            if (e.Value == null) return;

            var exerciseId = Convert.ToInt64(e.Value);

            var exercise = AvailableExercises.FirstOrDefault(x => x.Id == exerciseId);

            if (exercise != null && !selectedExercises.Any(x => x.Id == exerciseId))
            {
                selectedExercises.Add(exercise);
            }
        }


        private void RemoveExercise(long exerciseId)
        {
            selectedExercises.RemoveAll(x => x.Id == exerciseId);
        }



        private async Task SaveTemplate()
        {
            if (string.IsNullOrWhiteSpace(newTemplate.Name))
            {
                ToastService.ShowError("Template name is required");
                return;
            }

            newTemplate.ExerciseIds = selectedExercises.Select(x => x.Id).ToList();

            var success = await Flow.AddTemplate(newTemplate, JWT);

            if (success.RespCode == "200")
            {
                ToastService.ShowSuccess("Template created");
            }
            else
            {
                ToastService.ShowError("Failed to save template");
                return;
            }


            newTemplate = new TemplateUpsertDto();
            selectedExercises.Clear();
            isCreating = false;

            await OnInitializedAsync();
            StateHasChanged();
        }

        
        private async Task DeleteTemplate(TemplateListDto template)
        {
            var success = await Flow.DeleteTemplate(template.Id, JWT);

            if (success.RespCode == "200")
            {
                savedTemplates.Remove(template);
                ToastService.ShowSuccess("Template deleted");
            }
            else
            {
                ToastService.ShowError("Delete failed");
            }
        }


    }
}
