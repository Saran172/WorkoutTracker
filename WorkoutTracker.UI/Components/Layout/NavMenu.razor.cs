using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace WorkoutTracker.UI.Components.Layout
{
    public partial class NavMenu : IDisposable
    {
        protected override void OnInitialized()
        {
            NavService.OnChange += StateHasChanged;
            NavService.IsOpen = false;

            if (NavService.CurrentContent == null)
            {
                NavService.SetActiveMenu("Dashboard");
            }
        }
        public void Dispose()
        {
            NavService.OnChange -= StateHasChanged;
        }

        public async Task logout()
        {
            AuthenticationStateProvider.Logout();
            await Task.Yield();
            _NavigationManager.NavigateTo("/", true);
        }
    }
}
