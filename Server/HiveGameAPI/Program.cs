using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Services;
using Microsoft.AspNetCore.Hosting;
using QuickGraph.Algorithms.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));

//Services
builder.Services.AddScoped<IHiveGameService, HiveGameService>();

//Others
builder.Services.AddScoped<IHiveGraph, HiveGraph>();


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
