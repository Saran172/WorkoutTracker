using WorkoutTracker.UI.Modal;
using static WorkoutTracker.UI.Components.MainScreens.ActiveWorkout;

namespace WorkoutTracker.UI.Components.Services
{
    public class ToastService
    {
        public event Action? OnChange;

        public List<ToastMessage> Toasts { get; } = new();

        public void ShowSuccess(string message)
        {
            AddToast(message, "finish");
        }

        public void ShowError(string message)
        {
            AddToast(message, "error");
        }

        private void AddToast(string message, string type)
        {
            var toast = new ToastMessage
            {
                Message = message,
                Type = type
            };

            Toasts.Add(toast);
            NotifyStateChanged();

            _ = RemoveLater(toast);
        }

        private async Task RemoveLater(ToastMessage toast)
        {
            await Task.Delay(3000);
            Toasts.Remove(toast);
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
