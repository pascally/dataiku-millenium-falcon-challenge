using Backend.Application.Interfaces.Common;
using Backend.Application.Interfaces.Repository;
using Backend.Application.Services;
using Backend.Domain.UseCases;
using Backend.Infrastructure.Repository;
using Backend.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile(builder.Configuration["DefaultConfiguration:FilePath"], optional: true, reloadOnChange: true);
builder.Services.AddSingleton<IRoutesRepository>(new RoutesRepository($"{AppContext.BaseDirectory}{builder.Configuration["routes_db"]}"));
builder.Services.AddSingleton<IConfigFileReader, JsonFileReader>();
builder.Services.AddSingleton<OnboardComputerUsecases, OnboardComputerService>();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Logger.LogInformation("Starting the app");

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

app.MapPost("/milleniumfalcon", ([FromServices] OnboardComputerUsecases computer, string path) =>
{
    if (string.IsNullOrEmpty(path) || !computer.LoadMilleniumFalconDatas(path))
    {
        return Results.UnprocessableEntity("path is empty");
    }

    return Results.Ok();//good practice would require to return Created with link to item + item data
})
.WithOpenApi();

app.MapPost("/empire", ([FromServices] OnboardComputerUsecases computer, string path) =>
{
    if (string.IsNullOrEmpty(path) || !computer.LoadEmpireDatas(path))
    {
        return Results.UnprocessableEntity("path is empty");
    }

    return Results.Accepted();//good practice would require to return Created with link to item + item data
})
.WithOpenApi();

app.MapGet("/ComputeOdds", () =>
{

})
.WithOpenApi();

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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
