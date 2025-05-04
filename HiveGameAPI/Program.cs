using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Repositories;
using HiveGame.BusinessLogic.Services;
using HiveGame.BusinessLogic.Utils;
using HiveGame.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HiveGame.Managers;
using HiveGame.Handlers;
using log4net;
using Microsoft.AspNetCore.SignalR;
using HiveGame.DataAccess.Repositories;
using HiveGame.DataAccess.Context;

var builder = WebApplication.CreateBuilder(args);


// Configuration for CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy =>
    policy.SetIsOriginAllowed(origin =>
    {
        return true;
        // desktop client
        if (string.IsNullOrEmpty(origin))
        {
            return true;
        }
        // web client
        return origin == "http://localhost:59843";
    })
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
});

//Logging
builder.Services.AddLogging(
    x => x.AddLog4Net()
);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Sneaker API",
        Description = "API for retrieving sneakers"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter token JWT in format: Bearer {your token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSignalR(options =>
{
    options.AddFilter<HubFilter>();
});
// Register application services
builder.Services.AddScoped<IHiveGameService, HiveGameService>();
builder.Services.AddScoped<IMatchmakingService, MatchmakingService>();

// Register repositories
builder.Services.AddSingleton<IMatchmakingRepository, MatchmakingRepository>();
builder.Services.AddSingleton<IGameRepository, GameRepository>();

// Register factories
builder.Services.AddScoped<IInsectFactory, InsectFactory>();
builder.Services.AddScoped<IGameFactory, GameFactory>();

// Register utilities
builder.Services.AddScoped<ITokenUtils, TokenUtils>();
builder.Services.AddScoped<IHiveMoveValidator, HiveMoveValidator>();
builder.Services.AddScoped<IConnectionManager, ConnectionManager>();
builder.Services.AddScoped<IGameActionsHandler, GameActionsHandler>();
builder.Services.AddScoped<IGameConverter, GameConverter>();

// Database context
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddSingleton<MongoDBContext>();

// Configure JWT authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:AuthKey"] ?? throw new Exception("Auth key not found")))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/matchmakinghub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Configure routing to use lowercase URLs
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = false;
});


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sneaker API V1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowOrigin");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/matchmakinghub");

app.Run();
