using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Notifications.Client;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.NotificationHandlers.Client;

public class ClientMessageReceivedHandler(ILogger<ClientMessageReceivedHandler> logger,
                                          IMediator mediator) : INotificationHandler<ClientMessageReceivedNotification>
{
    public async Task Handle(ClientMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            var message = notification.Message;

            if (message.Channel is IDMChannel)
            {
                // Route to DirectMessageReceivedHandler
                await mediator.Publish(new DirectMessageReceivedNotification(message), cancellationToken);
            }
            else if (message.Channel is IGuildChannel)
            {
                // Route to GuildMessageReceivedHandler
                await mediator.Publish(new GuildMessageReceivedNotification(message), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occurred in ClientMessageReceivedHandler.");
        }
    }
}
