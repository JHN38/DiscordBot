using Discord.WebSocket;
using DiscordBot.Domain.DirectMessage.Events;
using MediatR;

namespace DiscordBot.Application.Discord.DirectMessage.Events;

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
