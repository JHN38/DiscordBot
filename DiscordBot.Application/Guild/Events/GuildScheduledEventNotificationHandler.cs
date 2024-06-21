using DiscordBot.Domain.Guild.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Guild.Events;

public class GuildScheduledEventNotificationHandler(ILogger<GuildScheduledEventNotificationHandler> logger) : INotificationHandler<GuildScheduledEventNotification>
{
    public async Task Handle(GuildScheduledEventNotification notification, CancellationToken cancellationToken)
    {
        var guildEvent = notification.GuildEvent;
        logger.LogInformation("Scheduled event created in guild {GuildName} (ID: {GuildId}) with event name {EventName} (ID: {EventId}). Starts at {StartTime}, ends at {EndTime}.",
            guildEvent.Guild.Name, guildEvent.Guild.Id, guildEvent.Name, guildEvent.Id, guildEvent.StartTime, guildEvent.EndTime);

        await Task.CompletedTask;
    }
}