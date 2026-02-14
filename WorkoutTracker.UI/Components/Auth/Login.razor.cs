using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using WorkoutTracker.UI.Components.Services;
using WorkoutTracker.UI.Services.AuthenticationStateProvide;

namespace WorkoutTracker.UI.Components.Auth
{
    public partial class Login  
    {

        [Parameter]
        public EventCallback OnBackToLanding { get; set; }
        [Parameter] 
        public EventCallback OnNavigateToSignUp { get; set; }
        public string? Email { get; set; } = "Saran@gmail.com";
        public string? Password { get; set; } = "saran";
        public bool rememberMe { get; set; } = false;
        private string PassInputType { get; set; }
        private bool passwordL { get; set; }
        public bool isLoading { get; set; } = false;
        public string? JWToken { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                _NavigationManager.NavigateTo("/");
            }
        }
        public async Task Login_Click()
        {
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Email))
            {
                return;
            }
            else
            {
                isLoading = true;
                var loginRes = await Flow.LoginAuth(Email, Password);
                if (loginRes.RespCode == "200")
                {
                    ToastService.ShowSuccess("Logged In!");
                    JWToken = loginRes.RespData.ToString();
                    var state = (CustomAuthenticationStateProvider)stateprovider;

                    StateHasChanged();

                    state.NotifyUserAuthentication(JWToken);

                    _NavigationManager.NavigateTo("/");
                    Password = "";
                    Email = "";
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
                    ToastService.ShowError("Login Failed");
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
