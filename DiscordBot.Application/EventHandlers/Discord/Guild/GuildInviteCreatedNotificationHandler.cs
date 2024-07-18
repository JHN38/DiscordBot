using DiscordBot.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.EventHandlers;

public class GuildInviteCreatedNotificationHandler(ILogger<GuildInviteCreatedNotificationHandler> logger) : INotificationHandler<GuildInviteCreatedNotification>
{
    public async Task Handle(GuildInviteCreatedNotification notification, CancellationToken cancellationToken)
    {
        var invite = notification.Invite;
        logger.LogInformation("Invite created in guild {GuildName} (ID: {GuildId}) for channel {ChannelName} (ID: {ChannelId}) by {Inviter} with code {InviteCode}.",
            invite.Guild.Name, invite.Guild.Id, invite.Channel.Name, invite.Channel.Id, invite.Inviter?.Username ?? "<Unknown>", invite.Code);

        await Task.CompletedTask;
    }
}