using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Bot.Core;
using DiscordBot.Bot.Core.Common.Helpers;
using DiscordBot.Bot.Core.Configurations;
using DiscordBot.Bot.Web.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using DiscordBot.Bot.Web.Services;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;
using Wolverine;
using Wolverine.Transports.Tcp;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Debug()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog((ctx, config) =>
{
    config.Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration);

    // Only configure Discord webhook sink if properly configured
    var webhookIdConfig = ctx.Configuration["SerilogWriteToDiscordWebhookId"];
    var webhookToken = ctx.Configuration["SerilogWriteToDiscordWebhookToken"];

    if (!string.IsNullOrWhiteSpace(webhookIdConfig) &&
        ulong.TryParse(webhookIdConfig, out var webhookId) &&
        !string.IsNullOrWhiteSpace(webhookToken))
    {
        config.WriteTo.Discord(webhookId, webhookToken, restrictedToMinimumLevel: LogEventLevel.Error);
    }
});

// Health checks
builder.Services.AddRequestTimeouts(
    configure: static timeouts =>
        timeouts.AddPolicy("HealthChecks", TimeSpan.FromSeconds(5)));

builder.Services.AddOutputCache(
    configureOptions: static caching =>
        caching.AddPolicy("HealthChecks",
        build: static policy => policy.Expire(TimeSpan.FromSeconds(10))));

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

// Environment variables
builder.Configuration.AddEnvironmentVariables("DBOT_");

// Load user secrets in development
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();

// Configuration
builder.Services.Configure<BotConfig>(builder.Configuration.GetSection("Bot"));
builder.Services.AddSingleton<IBotConfig>(s => s.GetRequiredService<IOptions<BotConfig>>().Value);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddMemoryCache();

// Discord client configuration
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
    .AddSingleton<IDiscordClient>(s => s.GetRequiredService<DiscordSocketClient>())
    .AddSingleton(s => new InteractionService(s.GetRequiredService<DiscordSocketClient>()))
    .AddHostedService<Worker>();

builder.Services.AddBotCore();

// Wolverine Configuration - service hosts are configurable for Docker
var recordKeepingHost = builder.Configuration["Wolverine:RecordKeepingHost"] ?? "localhost";
var weatherHost = builder.Configuration["Wolverine:WeatherHost"] ?? "localhost";
var webSearchHost = builder.Configuration["Wolverine:WebSearchHost"] ?? "localhost";

builder.Host.UseWolverine(opts =>
{
    // Disable scanning of all referenced assemblies, then explicitly include what we need
    opts.Discovery.IncludeAssembly(AssemblyReference.Assembly);

    // Configure TCP publishers for each service
    // RecordKeeping service - handles all command messages
    opts.Publish(rule =>
    {
        rule.MessagesFromNamespace("DiscordBot.Contracts.RecordKeeping");
        rule.ToServerAndPort(recordKeepingHost, 5001);
    });

    // Weather service - handles weather queries
    opts.Publish(rule =>
    {
        rule.MessagesFromNamespace("DiscordBot.Contracts.Weather");
        rule.ToServerAndPort(weatherHost, 5002);
    });

    // WebSearch service - handles web search queries
    opts.Publish(rule =>
    {
        rule.MessagesFromNamespace("DiscordBot.Contracts.WebSearch");
        rule.ToServerAndPort(webSearchHost, 5003);
    });
}, ExtensionDiscovery.ManualOnly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseStatusCodePages();

app.UseSerilogRequestLogging(opts =>
{
    opts.GetLevel = (ctx, _, _) =>
        ctx.Request.Path.StartsWithSegments("/health") || ctx.Request.Path.StartsWithSegments("/alive")
            ? LogEventLevel.Verbose
            : LogEventLevel.Information;
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRequestTimeouts();
app.UseOutputCache();

app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health")
    .CacheOutput("HealthChecks")
    .WithRequestTimeout("HealthChecks");

app.MapHealthChecks("/alive", new() { Predicate = _ => false })
    .CacheOutput("HealthChecks")
    .WithRequestTimeout("HealthChecks");

await app.RunAsync();
