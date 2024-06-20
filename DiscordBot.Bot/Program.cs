using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Application;
using DiscordBot.Application.Common.Configuration;
using DiscordBot.Bot.Services;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Load user secrets in development
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
}

// Add services to the container.
builder.Services.Configure<BotOptions>(builder.Configuration.GetSection("Bot"));

builder.Services.AddApplication();
builder.Services.AddSingleton((s) =>
    {
        var options = s.GetRequiredService<IOptions<BotOptions>>();

        return new DiscordSocketConfig
        {
            AlwaysDownloadUsers = options.Value.AlwaysDownloadUsers,
            DefaultRetryMode = RetryMode.AlwaysRetry,
            GatewayIntents = GatewayIntents.All,
#if DEBUG
            LogLevel = LogSeverity.Debug
#else
            LogLevel = LogSeverity.Warning
#endif
        };
    })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton(s => new InteractionService(s.GetRequiredService<DiscordSocketClient>()))
    .AddHostedService<Worker>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add serilog
builder.Host.UseSerilog((ctx, config) =>
        config.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

app.MapDefaultEndpoints();

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
