using System.Text.Json;
using DiscordBot.Domain.Guild.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Guild.Events;

public class GuildPresenceUpdateNotificationHandler(ILogger<GuildPresenceUpdateNotificationHandler> logger) : INotificationHandler<GuildPresenceUpdateNotification>
{
    public async Task Handle(GuildPresenceUpdateNotification notification, CancellationToken cancellationToken)
    {
        if (notification.OldPresence.Status == notification.NewPresence.Status)
            return;

        //logger.LogInformation("Presence updated for user {UserName}. Old status: {OldStatus}, new status: {NewStatus}.",
        //    notification.User.Username, notification.OldPresence.Status, notification.NewPresence.Status);

        await Task.CompletedTask;
    }
}
