using Discord;
using Discord.WebSocket;
using DiscordBot.Application.Events;
using DiscordBot.Infrastructure.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace DiscordBot.Bot.Services;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMediator _mediator;
    private readonly DiscordSocketClient _client;
    private readonly IOptions<BotConfig> _botOptions;
    private readonly IServiceScope _serviceScope;

    public Worker(ILogger<Worker> logger,
                DiscordSocketClient client,
                IOptions<BotConfig> botOptions,
                IServiceProvider serviceProvider)
    {
        _serviceScope = serviceProvider.CreateScope();
        _mediator = _serviceScope.ServiceProvider.GetRequiredService<IMediator>();
        _logger = logger;
        _client = client;
        _botOptions = botOptions;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _client.Log += async (message) => await _mediator.Publish(new ClientLogNotification(message), cancellationToken);
            _client.MessageReceived += async (message) => await _mediator.Publish(new ClientMessageReceivedNotification(message), cancellationToken);
            _client.Ready += async () => await _mediator.Publish(new ClientReadyNotification(), cancellationToken);
            _client.PresenceUpdated += async (user, oldPresence, newPresence) => await _mediator.Publish(new GuildPresenceUpdateNotification(user, oldPresence, newPresence), cancellationToken);
            _client.GuildScheduledEventCreated += async (guildEvent) => await _mediator.Publish(new GuildScheduledEventNotification(guildEvent), cancellationToken);
            _client.InviteCreated += async (invite) => await _mediator.Publish(new GuildInviteCreatedNotification(invite), cancellationToken);

            // process the InteractionCreated payloads to execute Interactions commands
            _client.InteractionCreated += async (socketInteration) => await _mediator.Publish(new ClientInteractionCreatedNotification(socketInteration), cancellationToken);

            await _client.LoginAsync(TokenType.Bot, _botOptions.Value.Token);
            await _client.StartAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An Exception occured while starting the service.");
        }

        _logger.LogInformation("Hosted Service started.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Service stopping.");
        _serviceScope.Dispose();

        return Task.CompletedTask;
    }
}
