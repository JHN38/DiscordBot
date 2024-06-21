using Discord.WebSocket;
using DiscordBot.Domain.Guild.Events;
using MediatR;

namespace DiscordBot.Application.Guild.Events;

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

        // General SocketMessage logic
    }
}
