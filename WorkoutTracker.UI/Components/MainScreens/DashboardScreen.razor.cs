namespace WorkoutTracker.UI.Components.MainScreens
{
    public partial class DashboardScreen
    {
        private void StartSession()
        {
            NavService.SetActiveMenu("ActiveWorkout");
            StateHasChanged();

        }
    }
}
