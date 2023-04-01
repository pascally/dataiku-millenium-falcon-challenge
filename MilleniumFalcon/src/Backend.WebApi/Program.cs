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


app.MapPost("/milleniumfalcon", (OnboardComputerUsecases computer, string path) =>
{
    if (string.IsNullOrEmpty(path) || !computer.LoadMilleniumFalconDatas(path))
    {
        return Results.UnprocessableEntity("path is empty");
    }

    return Results.Ok();//good practice would require to return Created with link to item + item data
})
.WithName("PostMilleniumFalconConfig")
.WithOpenApi();

app.MapPost("/empire", (OnboardComputerUsecases computer, string path) =>
{
    if (string.IsNullOrEmpty(path) || !computer.LoadEmpireDatas(path))
    {
        return Results.UnprocessableEntity("path is empty");
    }

    return Results.Accepted();//good practice would require to return Created with link to item + item data
})
.WithName("PostEmpireConfig")
.WithOpenApi();

app.MapGet("/successodds", (OnboardComputerUsecases computer) =>
{
    return Results.Ok(computer.ComputeOddsToDestination());
})
.WithName("GetSuccessOdds")
.WithOpenApi();

app.Run();
