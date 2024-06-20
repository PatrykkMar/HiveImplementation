using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Managers;
using HiveGame.BusinessLogic.Models.Game.Graph;
using HiveGame.BusinessLogic.Services;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

//Services
builder.Services.AddScoped<IHiveGameService, HiveGameService>();
builder.Services.AddScoped<IMatchmakingService, MatchmakingService>();

//Managers
builder.Services.AddSingleton<IWebSocketManager, HiveGame.BusinessLogic.Managers.WebSocketManager>();

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

app.Run();
