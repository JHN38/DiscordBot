using Discord;
using DiscordBot.Domain.Commands.Events;
using MediatR;

namespace DiscordBot.Application.Commands.Events;

public class MentionCommandNotificationHandler : INotificationHandler<MentionCommandNotification>
{
    public async Task Handle(MentionCommandNotification notification, CancellationToken cancellationToken)
    {
        await notification.Message.ReplyAsync("Pong!");
    }
}
