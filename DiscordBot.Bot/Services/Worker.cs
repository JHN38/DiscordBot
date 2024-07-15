using Discord;
using Discord.WebSocket;
using DiscordBot.Application.Common.Configuration;
using DiscordBot.Domain.Client.Events;
using DiscordBot.Domain.Guild.Events;
using MediatR;
using Microsoft.Extensions.Options;

namespace DiscordBot.Bot.Services;

public class Worker(ILogger<Worker> logger,
                    DiscordSocketClient client,
                    IOptions<BotConfig> botOptions,
                    IMediator mediator) : IHostedService
{
    public async Task StartAsync(CancellationToken stoppingToken)
    {
        try
        {
            client.Log += async (message) => await mediator.Publish(new ClientLogNotification(message), stoppingToken);
            client.MessageReceived += async (message) => await mediator.Publish(new ClientMessageReceivedNotification(message), stoppingToken);
            client.Ready += async () => await mediator.Publish(new ClientReadyNotification(), stoppingToken);
            client.PresenceUpdated += async (user, oldPresence, newPresence) => await mediator.Publish(new GuildPresenceUpdateNotification(user, oldPresence, newPresence), stoppingToken);
            client.GuildScheduledEventCreated += async (guildEvent) => await mediator.Publish(new GuildScheduledEventNotification(guildEvent), stoppingToken);
            client.InviteCreated += async (invite) => await mediator.Publish(new GuildInviteCreatedNotification(invite), stoppingToken);

            // process the InteractionCreated payloads to execute Interactions commands
            client.InteractionCreated += async (socketInteration) => await mediator.Publish(new ClientInteractionCreatedNotification(socketInteration), stoppingToken);

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
