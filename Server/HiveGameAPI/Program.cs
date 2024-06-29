using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Managers;
using HiveGame.BusinessLogic.Models.Game.Graph;
using HiveGame.BusinessLogic.Services;
using HiveGame.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSignalR();

//Services
builder.Services.AddScoped<IHiveGameService, HiveGameService>();

//Managers
builder.Services.AddSingleton<IPlayerConnectionManager, PlayerConnectionManager>();

//Factories
builder.Services.AddScoped<IInsectFactory, InsectFactory>();

//Others
builder.Services.AddScoped<HiveBoard, HiveBoard>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<GameHub>("gamehub");

app.Run();
