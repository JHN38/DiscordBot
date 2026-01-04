using DiscordBot.Service.WebSearch.Infrastructure;
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

builder.Services.AddWebSearchInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

var wolverineListenHost = builder.Configuration["Wolverine:ListenHost"] ?? "0.0.0.0";
var wolverineListenPort = builder.Configuration["Wolverine:ListenPort"] ?? "5003";

builder.Host.UseWolverine(opts =>
{
    // Listen on all interfaces to accept connections from other Docker containers
    opts.ListenForMessagesFrom(new Uri($"tcp://{wolverineListenHost}:{wolverineListenPort}"));
}, ExtensionDiscovery.ManualOnly);

var app = builder.Build();

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
