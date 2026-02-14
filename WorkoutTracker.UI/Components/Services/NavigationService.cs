using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Data;
using WorkoutTracker.UI.Components.MainScreens;

namespace WorkoutTracker.UI.Service
{
    public class NavigationService
    {
        private readonly IJSRuntime JS;
        public NavigationService(IJSRuntime js)
        {
            JS = js;
        }
        public async Task showError(string? content)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    await JS.InvokeVoidAsync("NotificationFx.error", "Message is Empty or NULL");
                }
                else
                {
                    await JS.InvokeVoidAsync("NotificationFx.error", content);
                }
            }
            catch (Exception ex)
            {
                
            }
            
        }
        public async Task showSuccess(string? content)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    await JS.InvokeVoidAsync("NotificationFx.error", "Message is Empty or NULL");
                }
                else
                {
                    await JS.InvokeVoidAsync("NotificationFx.success", content);
                }
            }
            catch (Exception ex)
            {

            }
            
                
        }
        public event Action? OnChange;
        public string ActiveMenu { get; set; } = "Dashboard";
        public RenderFragment? CurrentContent { get; private set; }

        // Map your menu names to Component Types
        private readonly Dictionary<string, Type> _routeMap = new()
        {
            { "Dashboard", typeof(DashboardScreen) },
            { "Workout", typeof(Workout) },
            { "ActiveWorkout", typeof(ActiveWorkout) },
            { "Planner", typeof(Planner) },
            { "Progress", typeof(Progress) },
            { "Profile", typeof(Profile) },
        };

        public void SetActiveMenu(string menu)
        {
            if (_routeMap.TryGetValue(menu, out var componentType))
            {
                ActiveMenu = menu;
                CurrentContent = builder =>
                {
                    builder.OpenComponent(0, componentType);
                    builder.SetKey(menu);
                    builder.CloseComponent();
                };

                NotifyStateChanged();
            }
        }
        private void NotifyStateChanged() => OnChange?.Invoke();
            public RenderFragment CreateComponent<T>() where T : IComponent => builder =>
            {
                builder.OpenComponent(0, typeof(T));
                builder.CloseComponent();
            };

        public bool IsOpen { get;  set; } = false;

        public void Toggle()
        {
            IsOpen = !IsOpen;
            NotifyStateChanged();
        }
    }
}
