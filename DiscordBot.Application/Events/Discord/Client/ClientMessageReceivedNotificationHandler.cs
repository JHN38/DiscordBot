using Discord;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Events;

public record ClientMessageReceivedNotification(IMessage Message) : INotification;


public class ClientMessageReceivedHandler(ILogger<ClientMessageReceivedHandler> logger,
                                          IMediator mediator) : INotificationHandler<ClientMessageReceivedNotification>
{
    public async Task Handle(ClientMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            var message = notification.Message;
            if (message.Author.IsBot) return;

            switch (message.Channel)
            {
                case IDMChannel:
                    await mediator.Publish(new DirectMessageReceivedNotification(message), cancellationToken);
                    break;

                case IGuildChannel:
                    await mediator.Publish(new GuildMessageReceivedNotification(message), cancellationToken);
                    break;

                default:
                    logger.LogWarning("Unknown channel type: {ChannelType}", message.Channel.GetType());
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occurred in ClientMessageReceivedHandler.");
        }
    }
}
