using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Graph;
using HiveGame.BusinessLogic.Repositories;
using HiveGame.BusinessLogic.Services;
using HiveGame.BusinessLogic.Utils;
using HiveGame.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSignalR();

//Services
//builder.Services.AddScoped<IHiveGameService, HiveGameService>();
builder.Services.AddScoped<IMatchmakingService, MatchmakingService>();

//Repository
builder.Services.AddScoped<IMatchmakingRepository, MatchmakingRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();

//Factories
builder.Services.AddScoped<IInsectFactory, InsectFactory>();
builder.Services.AddScoped<IGameFactory, GameFactory>();

//Others
builder.Services.AddScoped<ITokenUtils, TokenUtils>();


builder.Services
    .AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:AuthKey")))
        };
    });

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

app.MapHub<MatchmakingHub>("matchmakinghub");

app.Run();
