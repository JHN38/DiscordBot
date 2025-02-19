using Discord;
using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Application.Events;

public record DirectMessageReceivedNotification(IMessage Message) : INotification;

public class DirectMessageReceivedHandler(IMediator mediator) : INotificationHandler<DirectMessageReceivedNotification>
{
    public async Task Handle(DirectMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        if (message is SocketUserMessage userMessage)
        {
            await mediator.Publish(new DirectUserMessageReceivedNotification(userMessage), cancellationToken);
        }

        if (message is SocketSystemMessage systemMessage)
        {
            await mediator.Publish(new DirectSystemMessageReceivedNotification(systemMessage), cancellationToken);
        }

        // General SocketMessage logic
    }
}
