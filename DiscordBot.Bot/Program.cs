using System;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Application;
using DiscordBot.Application.Common.Helpers;
using DiscordBot.Bot;
using DiscordBot.Bot.Services;
using DiscordBot.Infrastructure;
using DiscordBot.Infrastructure.Configuration;
using DiscordBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Register health checks
builder.AddDefaultHealthChecks();

// Environment variables
builder.Configuration.AddEnvironmentVariables("DBOT_");

// Load user secrets in development
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
}

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Add services to the container.
builder.Services.AddSingleton((serviceProvider) =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<BotConfig>>().Value;
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var defaultLogLevel = LogLevelHelper.GetDefaultSerilogLogLevel(builder.Configuration) ?? LogEventLevel.Information;

        logger.LogInformation("Default log level is {DefaultLogLevel}", defaultLogLevel);

        return new DiscordSocketConfig
        {
            AlwaysDownloadUsers = options.AlwaysDownloadUsers,
            DefaultRetryMode = RetryMode.AlwaysRetry,
            GatewayIntents = GatewayIntents.All,
            LogLevel = defaultLogLevel.ConvertToDiscord()
        };
    })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton(s => new InteractionService(s.GetRequiredService<DiscordSocketClient>()))
    .AddHostedService<Worker>();

// Add the application services
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// Add serilog
builder.Host.UseSerilog((ctx, config) =>
    config.Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

// Wherever your services are being registered.
// Before the call to Build().
builder.Services.AddRequestTimeouts();
builder.Services.AddOutputCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    var dbContext = app.Services.GetRequiredService<AppDbContext>();
    await dbContext.Database.OpenConnectionAsync(); // Open the connection to the in-memory database
    await dbContext.Database.EnsureCreatedAsync();  // Ensure the database is created
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapDefaultEndpoints();

// Wherever your app has been built, before the call to Run().
app.UseRequestTimeouts();
app.UseOutputCache();

await app.RunAsync();
