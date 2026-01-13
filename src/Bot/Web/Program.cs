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
using Wolverine.RabbitMQ;

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

// RabbitMQ Configuration
var rabbitConfig = builder.Configuration
    .GetSection(RabbitMqConfig.SectionName)
    .Get<RabbitMqConfig>() ?? new RabbitMqConfig();

// Graceful shutdown configuration
builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

builder.Host.UseWolverine(opts =>
{
    // RabbitMQ transport configuration
    opts.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = rabbitConfig.Host;
        rabbit.Port = rabbitConfig.Port;
        rabbit.VirtualHost = rabbitConfig.VirtualHost;
        rabbit.UserName = rabbitConfig.Username;
        rabbit.Password = rabbitConfig.Password;
    })
    .AutoProvision()
    .UseConventionalRouting();

    // Publish RecordKeeping commands (fire-and-forget)
    opts.PublishAllMessages()
        .ToRabbitExchange("discordbot", exchange => exchange.ExchangeType = Wolverine.RabbitMQ.ExchangeType.Topic);

    // RPC reply handling - Wolverine automatically sets ReplyUri header
    opts.ListenToRabbitQueue("bot.replies")
        .ProcessInline();

    opts.Discovery.IncludeAssembly(AssemblyReference.Assembly);
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
