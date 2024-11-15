using Discord;
using Discord.WebSocket;
using DiscordBot.Application.Events;
using DiscordBot.Infrastructure.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace DiscordBot.Bot.Services;

public class Worker(ILogger<Worker> logger,
                    DiscordSocketClient client,
                    IOptions<BotConfig> botOptions,
                    IMediator mediator) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            client.Log += async (message) => await mediator.Publish(new ClientLogNotification(message), cancellationToken);
            client.MessageReceived += async (message) => await mediator.Publish(new ClientMessageReceivedNotification(message), cancellationToken);
            client.Ready += async () => await mediator.Publish(new ClientReadyNotification(), cancellationToken);
            client.PresenceUpdated += async (user, oldPresence, newPresence) => await mediator.Publish(new GuildPresenceUpdateNotification(user, oldPresence, newPresence), cancellationToken);
            client.GuildScheduledEventCreated += async (guildEvent) => await mediator.Publish(new GuildScheduledEventNotification(guildEvent), cancellationToken);
            client.InviteCreated += async (invite) => await mediator.Publish(new GuildInviteCreatedNotification(invite), cancellationToken);

            // process the InteractionCreated payloads to execute Interactions commands
            client.InteractionCreated += async (socketInteration) => await mediator.Publish(new ClientInteractionCreatedNotification(socketInteration), cancellationToken);

            await client.LoginAsync(TokenType.Bot, botOptions.Value.Token);
            await client.StartAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occured while starting the service.");
        }

        logger.LogInformation("Hosted Service started.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Hosted Service stopping.");

        return Task.CompletedTask;
    }
}
