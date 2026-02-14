using Shared.Entities;
using Shared.PlannerDTO;

namespace WorkoutTracker.UI.Components.Services
{
    public class WorkoutStateService
    {
        public TemplateDetailDto? PendingTemplate { get; set; }
        public void Clear() => PendingTemplate = null;
    }
}
