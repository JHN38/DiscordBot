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

// Add the application services
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add serilog
builder.Host.UseSerilog((ctx, config) =>
    config.Enrich.FromLogContext()
        //.WriteTo.OpenTelemetry(options =>
        //{
        //    options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]!;
        //    var headers = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]?.Split(',') ?? [];
        //    foreach (var header in headers)
        //    {
        //        var (key, value) = header.Split('=') switch
        //        {
        //            [string k, string v] => (k, v),
        //            var v => throw new Exception($"Invalid header format {v}")
        //        };

        //        options.Headers.Add(key, value);
        //    }
        //    options.ResourceAttributes.Add("service.name", "discordbot-bot");

        //    //To remove the duplicate issue, we can use the below code to get the key and value from the configuration
        //    var (otelResourceAttribute, otelResourceAttributeValue) = builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]?.Split('=') switch
        //    {
        //    [string k, string v] => (k, v),
        //        _ => throw new Exception($"Invalid header format {builder.Configuration["OTEL_RESOURCE_ATTRIBUTES"]}")
        //    };

        //    options.ResourceAttributes.Add(otelResourceAttribute, otelResourceAttributeValue);
        //})
        .ReadFrom.Configuration(ctx.Configuration)
);

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
