using Microsoft.AspNetCore.Components;

namespace WorkoutTracker.UI.Components.MainScreens
{
    public partial class LandingPage
    {
        [Parameter]
        public EventCallback OnGetStarted { get; set; }
        [Parameter]
        public EventCallback OnSignIn { get; set; }

    }
}
