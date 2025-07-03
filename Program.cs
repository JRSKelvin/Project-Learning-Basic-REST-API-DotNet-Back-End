using Microsoft.EntityFrameworkCore;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Data;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Interfaces;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Models.Config;
using Project_Learning_Basic_REST_API_DotNet_Back_End.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

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

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
.WithOpenApi();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
