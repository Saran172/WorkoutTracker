using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Learning.Services.Classes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using CommonServices.DataAccess;
using WorkoutTracker.API.DbCon;
using WorkoutTracker.API.DecrptionServices;
using WorkoutTracker.API.EncryptionServices;
using WorkoutTracker.API.GeneralServices;
using WorkoutTracker.API.LoginServices;
using WorkoutTracker.API.Swagger;
using WTAlgoServices.Classes;
using WTAlgoServices.InitializeService;
using WTAlgoServices.Interfaces;
using WTAlgoServices.Strategy;
using WTAlgoServices.TrainingFactory;
using WTAlgoServices.Wservices;
using ApiVersion = Asp.Versioning.ApiVersion;

var builder = WebApplication.CreateBuilder(args);

#region CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy
//            .AllowAnyOrigin()
//            .AllowAnyMethod()
//            .AllowAnyHeader();
//    });
//});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWorkoutTrackerUI", policy =>
    {
        policy
            .WithOrigins("https://workouttracker-112.runasp.net/")  
            .AllowAnyMethod()                                 
            .AllowAnyHeader()
            .AllowCredentials();                            
    });
});
#endregion

#region Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
#endregion

#region API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);

    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("api-version"),
        new QueryStringApiVersionReader("v")
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";   // v1, v2
    options.SubstituteApiVersionInUrl = true;
});
#endregion

#region Swagger config
builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Authentication", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                   {
                      new OpenApiSecurityScheme
                      {
                          Reference = new OpenApiReference
                          {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Authentication" // Reference the security definition by name
                          },
                      },
                      new string[]{}
                   }
                });

});

builder.Services.ConfigureOptions<SwaggerConfigOption>();
#endregion

#region Database
builder.Services.AddDbContext<WTContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Dependency Injection
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<ITokenDecryption, TokenDecryption>();
builder.Services.AddScoped<IEnryptService, EncryptionService>();
builder.Services.AddScoped<DataAcces>();
builder.Services.AddScoped<IGeneralService, GeneralService>();

builder.Services.AddScoped<HypertrophyProgressionStrategy>();
builder.Services.AddScoped<StrengthProgressionStrategy>();
builder.Services.AddScoped<IProgressionStrategyFactory, ProgressionStrategyFactory>();

builder.Services.AddScoped<IStimulusCalculator, StimulusCalculator>();
builder.Services.AddScoped<IVolumeCalculator, VolumeCalculator>();
builder.Services.AddScoped<WorkoutService>();
builder.Services.AddScoped<WorkoutInitializationService>();
#endregion

#region JWT Authentication
var authKey = builder.Configuration["JwtSettings:SecurityKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
#endregion

var app = builder.Build();

app.UseCors("AllowWorkoutTrackerUI");

var apiVersionProvider =
    app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

#region Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
});


app.MapControllers();
#endregion

app.Run();

