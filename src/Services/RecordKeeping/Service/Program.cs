using DiscordBot.Service.RecordKeeping.Configuration;
using DiscordBot.Service.RecordKeeping.Messaging;
using DiscordBot.Service.RecordKeeping.Persistence;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Wolverine;
using Wolverine.RabbitMQ;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateSlimBuilder(args);

builder.Host.UseSerilog((ctx, config) =>
    config.Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

builder.Configuration.AddEnvironmentVariables("DBOT_");

builder.Services.AddPersistence(builder.Configuration, builder.Environment);
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

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
    .AutoProvision();

    // Listen to commands queue - supports competing consumers
    // Wolverine auto-provisions the queue; use conventional routing for message dispatch
    opts.ListenToRabbitQueue(RabbitMqTopology.CommandsQueue)
        .PreFetchCount((ushort)rabbitConfig.PrefetchCount)
        .UseDurableInbox();


}, ExtensionDiscovery.ManualOnly);

var app = builder.Build();

// Apply migrations with retry logic for container orchestration
await ApplyMigrationsWithRetryAsync(app.Services, app.Logger, maxRetries: 10, delaySeconds: 5);

app.UseSerilogRequestLogging(opts =>
{
    opts.GetLevel = (ctx, _, _) =>
        ctx.Request.Path.StartsWithSegments("/health") || ctx.Request.Path.StartsWithSegments("/alive")
            ? LogEventLevel.Verbose
            : LogEventLevel.Information;
});

app.MapHealthChecks("/health");
app.MapHealthChecks("/alive", new() { Predicate = _ => false });

await app.RunAsync();

static async Task ApplyMigrationsWithRetryAsync(IServiceProvider services, Microsoft.Extensions.Logging.ILogger logger, int maxRetries, int delaySeconds)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            logger.LogInformation("Attempting database migration (attempt {Attempt}/{MaxRetries})...", attempt, maxRetries);

            await dbContext.Database.MigrateAsync();

            logger.LogInformation("Database migration completed successfully");
            return;
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            logger.LogWarning(ex, "Database migration failed (attempt {Attempt}/{MaxRetries}). Retrying in {Delay} seconds...",
                attempt, maxRetries, delaySeconds);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }

    // Final attempt - let exception propagate
    using (var scope = services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
