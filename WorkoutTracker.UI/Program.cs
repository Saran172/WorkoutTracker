using CommonServices.Implementation;
using CommonServices.Interface;
using Microsoft.AspNetCore.Components.Authorization;
using WorkoutTracker.UI.Components;
using WorkoutTracker.UI.Components.Services;
using WorkoutTracker.UI.Service;
using WorkoutTracker.UI.Services.AuthenticationStateProvide;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IDataflow, Dataflow>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<NavigationService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<WorkoutStateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();


app.UseAuthentication();
app.UseAuthorization();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
