using DiscordBot.Domain.Events;
using MediatR;

namespace DiscordBot.Application.EventHandlers;

public class GuildPresenceUpdateNotificationHandler : INotificationHandler<GuildPresenceUpdateNotification>
{
    public async Task Handle(GuildPresenceUpdateNotification notification, CancellationToken cancellationToken)
    {
        if (notification.OldPresence.Status == notification.NewPresence.Status)
            return;

        await Task.CompletedTask;
    }
}
