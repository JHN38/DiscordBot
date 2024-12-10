using Discord;
using MediatR;

namespace DiscordBot.Application.Events;

public record GuildMessageReceivedNotification(IMessage Message) : INotification;

public class GuildMessageReceivedNotificationHandler(IMediator mediator) : INotificationHandler<GuildMessageReceivedNotification>
{
    public async Task Handle(GuildMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        if (message is IUserMessage userMessage)
        {
            await mediator.Publish(new GuildUserMessageReceivedNotification(userMessage), cancellationToken);
        }

        if (message is ISystemMessage systemMessage)
        {
            await mediator.Publish(new GuildSystemMessageReceivedNotification(systemMessage), cancellationToken);
        }
    }
}
