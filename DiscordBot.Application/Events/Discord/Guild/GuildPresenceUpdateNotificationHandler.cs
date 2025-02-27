﻿using Discord;
using MediatR;

namespace DiscordBot.Application.Events;

public record GuildPresenceUpdateNotification(IUser User, IPresence OldPresence, IPresence NewPresence) : INotification;

public class GuildPresenceUpdateNotificationHandler : INotificationHandler<GuildPresenceUpdateNotification>
{
    public async Task Handle(GuildPresenceUpdateNotification notification, CancellationToken cancellationToken)
    {
        if (notification.OldPresence.Status == notification.NewPresence.Status)
            return;

        await Task.CompletedTask;
    }
}
