using Discord;
using DiscordBot.Domain.Client.Events;
using DiscordBot.Domain.DirectMessage.Events;
using DiscordBot.Domain.Guild.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Discord.Client.Events;

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

                case IGuildChannel guildChannel:
                    await mediator.Publish(new GuildMessageReceivedNotification(message), cancellationToken);
                    break;

                default:
                    logger.LogWarning("Unknown channel type: {channelType}", message.Channel.GetType());
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occurred in ClientMessageReceivedHandler.");
        }
    }
}
