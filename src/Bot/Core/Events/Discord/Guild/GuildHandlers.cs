using System.Text.RegularExpressions;
using Discord;
using DiscordBot.Bot.Core.Commands.TextCommands;
using DiscordBot.Bot.Core.Configurations;
using DiscordBot.Bot.Core.Events.Discord.Client;
using DiscordBot.Contracts.RecordKeeping;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace DiscordBot.Bot.Core.Events.Discord.Guild;

using DiscordBot.Contracts.RecordKeeping;

public sealed partial class GuildUserMessageReceivedHandler(
    ILogger<GuildUserMessageReceivedHandler> logger,
    IMessageBus bus,
    IBotConfig config)
{
    [GeneratedRegex("""
        ^(?<cmd>weather|w)(?<subcmd>forecast|f)?\s+(?<arg>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex WeatherCommandRegex();

    [GeneratedRegex("""
        ^(?<cmd>search|s)(?<count>\d)?(?<cr>\w{2})?\s+(?<arg>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex SearchCommandRegex();

    [GeneratedRegex("""
        ^(?<cmd>msg|m)\s+(?<action>\w+)\s+(?<args>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex MessageCommandRegex();

    [GeneratedRegex("""
        ^(?<cmd>database|db)\s+(?<action>get|g)\s+(?<args>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex DatabaseCommandRegex();

    [GeneratedRegex("""
        ^(?<cmd>bot|b)\s+(?<action>\w+)\s+(?<args>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex BotCommandRegex();

    public async Task Handle(ClientMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.Message is not IUserMessage message ||
            message.Channel is not IGuildChannel ||
            message.Author.IsBot)
            return;

        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;

        var author = await guild.GetUserAsync(message.Author.Id);

        logger.LogInformation("({Guild}) #{Channel} <{User}> {Message}",
            guild.Name, guildChannel.Name, author.DisplayName, message.CleanContent);

        // Save message to RecordKeeping service
        await bus.PublishAsync(new SaveMessageCommand(
            message.Id,
            message.Channel.Id,
            message.Channel.Name,
            guild.Id,
            guild.Name,
            message.Author.Id,
            message.Author.Username,
            message.Content,
            message.Timestamp,
            message.Author.IsBot,
            message.Author.GlobalName,
            message.Author.GetAvatarUrl()));

        if (config.TextCommandPrefix is string prefix && message.Content.StartsWith(prefix, StringComparison.Ordinal))
        {
            await HandleTextCommandAsync(message, message.Content[prefix.Length..], cancellationToken);
            return;
        }

        var botUser = await guildChannel.Guild.GetCurrentUserAsync();
        if (message.Content.StartsWith(botUser.Mention, StringComparison.Ordinal))
        {
            logger.LogInformation("Bot mentioned in message: {Message}", message.Content);
        }
    }

    private async Task HandleTextCommandAsync(IUserMessage message, string command, CancellationToken cancellationToken)
    {
        logger.LogDebug("Processing text command: {Message}", message.Content);

        if (WeatherCommandRegex().Match(command) is { Success: true } weatherCommandMatch)
        {
            var subCommand = weatherCommandMatch.Groups["subcmd"].Value;
            var arg = weatherCommandMatch.Groups["arg"].Value;
            await bus.InvokeAsync<IUserMessage>(new WeatherCommand(message, subCommand, arg), cancellationToken);
            return;
        }

        if (SearchCommandRegex().Match(command) is { Success: true } searchCommandMatch)
        {
            var country = searchCommandMatch.Groups["cr"].Value;
            var arg = searchCommandMatch.Groups["arg"].Value;
            var resultCount = searchCommandMatch.Groups["count"].Value switch
            {
                var s when int.TryParse(s, out var n) => Math.Clamp(n, 1, 9),
                _ => 1
            };
            await bus.InvokeAsync<IUserMessage>(new WebSearchCommand(message, arg, resultCount, country), cancellationToken);
            return;
        }

        if (MessageCommandRegex().Match(command) is { Success: true } messageCommandMatch)
        {
            var action = messageCommandMatch.Groups["action"].Value;
            var args = messageCommandMatch.Groups["args"].Value
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            await bus.InvokeAsync<IUserMessage>(new MessageCommand(message, action, args), cancellationToken);
            return;
        }

        if (DatabaseCommandRegex().Match(command) is { Success: true } databaseCommandMatch)
        {
            var action = databaseCommandMatch.Groups["action"].Value;
            var args = databaseCommandMatch.Groups["args"].Value
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            await bus.InvokeAsync<IUserMessage>(new DatabaseCommand(message, action, args), cancellationToken);
            return;
        }

        if (BotCommandRegex().Match(command) is { Success: true } botCommandMatch)
        {
            var action = botCommandMatch.Groups["action"].Value;
            var args = botCommandMatch.Groups["args"].Value
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            await bus.InvokeAsync<IUserMessage>(new BotCommand(message, action, args), cancellationToken);
            return;
        }

        logger.LogWarning("Unknown command: {Command}", command);
    }
}

public sealed class GuildSystemMessageReceivedNotificationHandler(ILogger<GuildSystemMessageReceivedNotificationHandler> logger)
{
    public Task Handle(ClientMessageReceivedNotification notification, CancellationToken _)
    {
        if (notification.Message is not ISystemMessage message ||
            message.Channel is not IGuildChannel)
            return Task.CompletedTask;

        var guildChannel = (IGuildChannel)message.Channel;

        logger.LogInformation("SYSTEM: ({Guild}) #{Channel} => {Message}",
            guildChannel.Guild.Name, guildChannel.Name, message.Content);

        return Task.CompletedTask;
    }
}

public sealed class GuildPresenceUpdateNotificationHandler(
    ILogger<GuildPresenceUpdateNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildPresenceUpdateNotification notification, CancellationToken _)
    {
        logger.LogDebug("PRESENCE: {User} changed status from {OldStatus} to {NewStatus}",
            notification.User.Username,
            notification.OldPresence?.Status,
            notification.NewPresence?.Status);

        var newPresence = notification.NewPresence;
        var oldPresence = notification.OldPresence;

        if (newPresence is null)
            return;

        var firstActivity = newPresence.Activities.FirstOrDefault();
        string? activityUrl = null;
        string? customEmoji = null;

        // Extract custom status emoji if available
        if (firstActivity is global::Discord.CustomStatusGame customStatus)
        {
            customEmoji = customStatus.Emote?.Name;
        }

        await bus.PublishAsync(new SavePresenceLogCommand(
            notification.User.Id,
            (int)newPresence.Status,
            DateTimeOffset.UtcNow,
            GuildId: null,
            PreviousStatus: oldPresence is not null ? (int)oldPresence.Status : null,
            ClientStatus: null,
            ActivityType: firstActivity?.Type is { } type ? (int)type : null,
            ActivityName: firstActivity?.Name,
            ActivityDetails: firstActivity?.Details,
            ActivityState: null,
            ActivityUrl: activityUrl,
            CustomStatusEmoji: customEmoji));
    }
}

public sealed class GuildScheduledEventNotificationHandler(
    ILogger<GuildScheduledEventNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildScheduledEventNotification notification, CancellationToken _)
    {
        logger.LogInformation("SCHEDULED EVENT: {EventName} in {Guild}",
            notification.GuildEvent.Name,
            notification.GuildEvent.Guild.Name);

        var guildEvent = notification.GuildEvent;

        await bus.PublishAsync(new SaveScheduledEventCommand(
            guildEvent.Id,
            guildEvent.Name,
            guildEvent.Guild.Id,
            guildEvent.StartTime,
            (int)guildEvent.PrivacyLevel,
            (int)guildEvent.Status,
            (int)guildEvent.Type,
            Description: guildEvent.Description,
            ScheduledEndTime: guildEvent.EndTime,
            EntityMetadata: guildEvent.Location,
            UserCount: guildEvent.UserCount,
            CoverImageUrl: guildEvent.GetCoverImageUrl(),
            ChannelId: guildEvent.Channel?.Id,
            CreatorId: guildEvent.Creator?.Id));
    }
}

public sealed class GuildInviteCreatedNotificationHandler(
    ILogger<GuildInviteCreatedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildInviteCreatedNotification notification, CancellationToken _)
    {
        logger.LogInformation("INVITE: {Code} created by {Inviter} for {Guild}",
            notification.Invite.Code,
            notification.Invite.Inviter?.Username ?? "Unknown",
            notification.Invite.Guild?.Name ?? "Unknown");

        var invite = notification.Invite;

        if (invite.Guild is null || invite.Channel is null)
            return;

        // Generate a hash-based ID for the invite since invite.Id might not be available
        var inviteIdHash = (ulong)invite.Code.GetHashCode(StringComparison.Ordinal);

        await bus.PublishAsync(new SaveInviteCommand(
            inviteIdHash,
            invite.Code,
            invite.Guild.Id,
            invite.Channel.Id,
            InviterId: invite.Inviter?.Id,
            MaxAge: invite.MaxAge,
            MaxUses: invite.MaxUses,
            Uses: invite.Uses,
            IsTemporary: invite.IsTemporary,
            ExpiresAt: invite.ExpiresAt,
            TargetType: invite.TargetUserType is { } type ? (int)type : null,
            TargetUserId: invite.TargetUser?.Id,
            TargetApplicationId: invite.Application?.Id));
    }
}

public sealed class GuildAvailableNotificationHandler(
    ILogger<GuildAvailableNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildAvailableNotification notification, CancellationToken _)
    {
        logger.LogInformation("GUILD AVAILABLE: {Guild} ({GuildId}) with {MemberCount} members",
            notification.Guild.Name,
            notification.Guild.Id,
            notification.Guild.MemberCount);

        var guild = notification.Guild;
        await bus.PublishAsync(new SaveGuildCommand(guild.Id, guild.Name));

        foreach (var channel in guild.Channels)
        {
            await bus.PublishAsync(new SaveChannelCommand(channel.Id, channel.Name, guild.Id, guild.Name));
        }
    }
}

public sealed class GuildMembersDownloadedNotificationHandler(
    ILogger<GuildMembersDownloadedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildMembersDownloadedNotification notification, CancellationToken _)
    {
        foreach (var user in notification.Guild.Users)
        {
            await bus.PublishAsync(new SaveUserCommand(
                user.Id,
                user.Username,
                user.Discriminator,
                user.IsBot,
                user.GlobalName,
                user.GetAvatarUrl()));
            
            logger.LogDebug("GUILD MEMBER DOWNLOADED: {Guild} {UserName} ({GlobalName}) {IsBot}",
                notification.Guild.Name,
                user.Username,
                user.GlobalName,
                user.IsBot ? "<Bot>" : "");
        }

        logger.LogInformation("GUILD MEMBERS DOWNLOADED: {Guild} ({GuildId}) - {DownloadedCount} members cached",
            notification.Guild.Name,
            notification.Guild.Id,
            notification.Guild.DownloadedMemberCount);
    }
}

public sealed class GuildUpdatedNotificationHandler(IMessageBus bus)
{
    public async Task Handle(GuildUpdatedNotification notification, CancellationToken _)
        => await bus.PublishAsync(new SaveGuildCommand(notification.After.Id, notification.After.Name));
}

public sealed class GuildUserJoinedNotificationHandler(IMessageBus bus)
{
    public async Task Handle(GuildUserJoinedNotification notification, CancellationToken _)
        => await bus.PublishAsync(new SaveUserCommand(
            notification.User.Id,
            notification.User.Username,
            notification.User.Discriminator,
            notification.User.IsBot,
            notification.User.GlobalName,
            notification.User.GetAvatarUrl()));
}

public sealed class GuildMemberUpdatedNotificationHandler(IMessageBus bus)
{
    public async Task Handle(GuildMemberUpdatedNotification notification, CancellationToken _)
    {
        var after = notification.After;

        // Save user info
        await bus.PublishAsync(new SaveUserCommand(
            after.Id,
            after.Username,
            after.Discriminator,
            after.IsBot,
            after.GlobalName,
            after.GetAvatarUrl()));

        // Track nickname changes
        if (notification.Before.HasValue)
        {
            var before = notification.Before.Value;
            if (before.Nickname != after.Nickname)
            {
                await bus.PublishAsync(new SaveNicknameCommand(
                    after.Id,
                    after.Guild.Id,
                    after.Nickname,
                    PreviousNickname: before.Nickname,
                    ChangedById: null));
            }
        }
    }
}

public sealed class GuildRoleCreatedNotificationHandler(
    ILogger<GuildRoleCreatedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildRoleCreatedNotification notification, CancellationToken _)
    {
        var role = notification.Role;

        logger.LogInformation("ROLE CREATED: {Role} in {Guild}",
            role.Name, role.Guild.Name);

        await bus.PublishAsync(new SaveRoleCommand(
            role.Id,
            role.Guild.Id,
            role.Name,
            (int)role.Color.RawValue,
            role.IsHoisted,
            role.Position,
            role.Permissions.RawValue,
            role.IsManaged,
            role.IsMentionable,
            IconUrl: role.GetIconUrl(),
            UnicodeEmoji: role.Emoji?.Name,
            Tags: role.Tags is not null ? System.Text.Json.JsonSerializer.Serialize(role.Tags) : null));
    }
}

public sealed class GuildRoleUpdatedNotificationHandler(
    ILogger<GuildRoleUpdatedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildRoleUpdatedNotification notification, CancellationToken _)
    {
        var role = notification.After;

        logger.LogInformation("ROLE UPDATED: {Role} in {Guild}",
            role.Name, role.Guild.Name);

        await bus.PublishAsync(new SaveRoleCommand(
            role.Id,
            role.Guild.Id,
            role.Name,
            (int)role.Color.RawValue,
            role.IsHoisted,
            role.Position,
            role.Permissions.RawValue,
            role.IsManaged,
            role.IsMentionable,
            IconUrl: role.GetIconUrl(),
            UnicodeEmoji: role.Emoji?.Name,
            Tags: role.Tags is not null ? System.Text.Json.JsonSerializer.Serialize(role.Tags) : null));
    }
}

public sealed class GuildRoleDeletedNotificationHandler(
    ILogger<GuildRoleDeletedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildRoleDeletedNotification notification, CancellationToken _)
    {
        var role = notification.Role;

        logger.LogInformation("ROLE DELETED: {Role} in {Guild}",
            role.Name, role.Guild.Name);

        await bus.PublishAsync(new DeleteRoleCommand(role.Id, DateTimeOffset.UtcNow));
    }
}

public sealed class GuildUserBannedNotificationHandler(
    ILogger<GuildUserBannedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildUserBannedNotification notification, CancellationToken _)
    {
        logger.LogInformation("USER BANNED: {User} from {Guild}",
            notification.User.Username, notification.Guild.Name);

        await bus.PublishAsync(new SaveBanCommand(
            notification.User.Id,
            notification.Guild.Id,
            DateTimeOffset.UtcNow,
            Reason: null,
            BannedById: null));
    }
}

public sealed class GuildUserUnbannedNotificationHandler(
    ILogger<GuildUserUnbannedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildUserUnbannedNotification notification, CancellationToken _)
    {
        logger.LogInformation("USER UNBANNED: {User} from {Guild}",
            notification.User.Username, notification.Guild.Name);

        await bus.PublishAsync(new UpdateBanCommand(
            notification.User.Id,
            notification.Guild.Id,
            DateTimeOffset.UtcNow,
            UnbannedById: null));
    }
}

public sealed class GuildUserVoiceStateUpdatedNotificationHandler(
    ILogger<GuildUserVoiceStateUpdatedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(GuildUserVoiceStateUpdatedNotification notification, CancellationToken _)
    {
        var before = notification.Before;
        var after = notification.After;
        var user = notification.User;

        // User joined a voice channel
        if (before.VoiceChannel is null && after.VoiceChannel is not null)
        {
            logger.LogInformation("USER JOINED VOICE: {User} joined {Channel}",
                user.Username, after.VoiceChannel.Name);

            await bus.PublishAsync(new SaveVoiceSessionCommand(
                after.VoiceSessionId ?? Guid.NewGuid().ToString(),
                user.Id,
                after.VoiceChannel.Id,
                after.VoiceChannel.Guild.Id,
                DateTimeOffset.UtcNow,
                IsSelfMuted: after.IsSelfMuted,
                IsSelfDeafened: after.IsSelfDeafened,
                IsServerMuted: after.IsMuted,
                IsServerDeafened: after.IsDeafened,
                IsStreaming: after.IsStreaming,
                IsVideoEnabled: after.IsVideoing,
                IsSuppressed: after.IsSuppressed,
                RequestToSpeakTimestamp: after.RequestToSpeakTimestamp));
        }
        // User left a voice channel
        else if (before.VoiceChannel is not null && after.VoiceChannel is null)
        {
            logger.LogInformation("USER LEFT VOICE: {User} left {Channel}",
                user.Username, before.VoiceChannel.Name);

            if (before.VoiceSessionId is not null)
            {
                await bus.PublishAsync(new UpdateVoiceSessionCommand(
                    before.VoiceSessionId,
                    DateTimeOffset.UtcNow));
            }
        }
        // User switched channels
        else if (before.VoiceChannel is not null && after.VoiceChannel is not null &&
                 before.VoiceChannel.Id != after.VoiceChannel.Id)
        {
            logger.LogInformation("USER SWITCHED VOICE: {User} from {OldChannel} to {NewChannel}",
                user.Username, before.VoiceChannel.Name, after.VoiceChannel.Name);

            // Close old session
            if (before.VoiceSessionId is not null)
            {
                await bus.PublishAsync(new UpdateVoiceSessionCommand(
                    before.VoiceSessionId,
                    DateTimeOffset.UtcNow));
            }

            // Open new session
            await bus.PublishAsync(new SaveVoiceSessionCommand(
                after.VoiceSessionId ?? Guid.NewGuid().ToString(),
                user.Id,
                after.VoiceChannel.Id,
                after.VoiceChannel.Guild.Id,
                DateTimeOffset.UtcNow,
                IsSelfMuted: after.IsSelfMuted,
                IsSelfDeafened: after.IsSelfDeafened,
                IsServerMuted: after.IsMuted,
                IsServerDeafened: after.IsDeafened,
                IsStreaming: after.IsStreaming,
                IsVideoEnabled: after.IsVideoing,
                IsSuppressed: after.IsSuppressed,
                RequestToSpeakTimestamp: after.RequestToSpeakTimestamp));
        }
    }
}
