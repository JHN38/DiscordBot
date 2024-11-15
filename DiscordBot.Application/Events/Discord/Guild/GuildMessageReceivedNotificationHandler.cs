using Discord;
using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Application.Events;

public record GuildMessageReceivedNotification(IMessage Message) : INotification;

public class GuildMessageReceivedNotificationHandler(IMediator mediator) : INotificationHandler<GuildMessageReceivedNotification>
{
    public async Task Handle(GuildMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        if (message is SocketUserMessage userMessage)
        {
            await mediator.Publish(new GuildUserMessageReceivedNotification(userMessage), cancellationToken);
        }

        if (message is SocketSystemMessage systemMessage)
        {
            await mediator.Publish(new GuildSystemMessageReceivedNotification(systemMessage), cancellationToken);
        }
    }
}
