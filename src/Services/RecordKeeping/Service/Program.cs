using DiscordBot.Service.RecordKeeping.Persistence;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Wolverine;
using Wolverine.Transports.Tcp;

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

var wolverineListenHost = builder.Configuration["Wolverine:ListenHost"] ?? "0.0.0.0";
var wolverineListenPort = builder.Configuration["Wolverine:ListenPort"] ?? "5001";

builder.Host.UseWolverine(opts =>
{
    // Listen on all interfaces to accept connections from other Docker containers
    opts.ListenForMessagesFrom(new Uri($"tcp://{wolverineListenHost}:{wolverineListenPort}"));
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
