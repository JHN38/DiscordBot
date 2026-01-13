using DiscordBot.Service.Weather.Configuration;
using DiscordBot.Service.Weather.Infrastructure;
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

builder.Services.AddWeatherInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

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

    // Listen to weather requests queue
    opts.ListenToRabbitQueue("weather.requests")
        .PreFetchCount((ushort)rabbitConfig.PrefetchCount)
        .UseDurableInbox();


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
