using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Data;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Interfaces;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Middleware;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Config;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Services;

var builder = WebApplication.CreateBuilder(args);

/* Retrieve Configuration From App Setting Local File */
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

/* Configure Dependency Injection For Database Setting */
builder.Services.Configure<DatabaseSetting>(builder.Configuration.GetSection("DatabaseSetting"));

var dbSetting = builder.Configuration.GetSection("DatabaseSetting").Get<DatabaseSetting>();

if (dbSetting is null || string.IsNullOrWhiteSpace(dbSetting.ConnectionString))
{
    Console.WriteLine("⚠️ Database Setting Or Connection String Is Not Configured And App Will Run In Dummy Mode");
}
else
{
    builder.Services.AddDbContext<AppDatabaseContext>((options) => options.UseNpgsql(dbSetting.ConnectionString));
    Console.WriteLine("⚠️ Database Setting Or Connection String Is Configured And App Will Try To Run In Database Mode");
}

/* Configure Dependency Injection For JWT Setting */

builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtSetting"));

var jwtSetting = builder.Configuration.GetSection("JwtSetting").Get<JwtSetting>();

if (jwtSetting is null || string.IsNullOrWhiteSpace(jwtSetting.AccessSecret))
{
    throw new InvalidOperationException("JWT Setting Or Access Secret Is Not Configured");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer((options) =>
{
    var key = Encoding.UTF8.GetBytes(jwtSetting.AccessSecret);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSetting.Issuer,
        ValidAudience = jwtSetting.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = (context) =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                statusCode = 401,
                title = "Unauthorized",
                message = "Authorization Token Is Required",
            });
            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization();

/* Register Additional Service */

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddLogging();

/* Register Additional Service */

builder.Services.AddSingleton<AppModeUserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithTags("App Example")
.WithOpenApi();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
