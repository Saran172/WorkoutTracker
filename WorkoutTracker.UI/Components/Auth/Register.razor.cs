using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using WorkoutTracker.UI.Components.Services;
using WorkoutTracker.UI.Services.AuthenticationStateProvide;

namespace WorkoutTracker.UI.Components.Auth
{
    public partial class Register
    {
        [Parameter] 
        public EventCallback OnBackToLogin { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public bool rememberMe { get; set; } = false;
        private string PassInputType { get; set; }
        public bool isLoading { get; set; } = false;
        public string? JWToken { get; set; }

        public async Task Register_Click()
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(FullName))
            {
                ToastService.ShowError("All fields are required");
                return;
            }
            else
            {
                isLoading = true;
                var loginRes = await Flow.Register(FullName,Email, Password);
                var loginRes1 = await Flow.LoginAuth(Email, Password);
                if(loginRes.RespCode == "200")
                {
                    if (loginRes1.RespCode == "200")
                    {
                        ToastService.ShowSuccess("Account created! Welcome to your fitness journey");
                        JWToken = loginRes1.RespData.ToString();
                        var state = (CustomAuthenticationStateProvider)stateprovider;

                        StateHasChanged();

                        state.NotifyUserAuthentication(JWToken);

                        _NavigationManager.NavigateTo("/");
                        Password = "";
                        Email = "";
                    }
                }
                else if (loginRes.RespCode == "500")
                {
                    ToastService.ShowError("Internal Server Error");
                    StateHasChanged();
                }
                else if (loginRes.RespCode == "409")
                {
                    ToastService.ShowError(loginRes.RespMessage);
                    StateHasChanged();
                }
                else if (loginRes.RespCode != "200")
                {
                    ToastService.ShowError("Registration Failed");
                    StateHasChanged();
                }

                else
                {
                    ToastService.ShowError("An unexpected error occurred");
                    StateHasChanged();
                }
                StateHasChanged();
            }
        }

        private async void togglepassword()
        {
            PassInputType = await Js.InvokeAsync<string>("TogglePass", "password");
            StateHasChanged();
        }
    }
}
