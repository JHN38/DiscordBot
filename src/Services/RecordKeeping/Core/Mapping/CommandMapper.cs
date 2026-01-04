using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for converting contract commands to internal DTOs.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class CommandMapper
{
    /// <summary>
    /// Maps a SaveMessageCommand to a DiscordMessageDto with nested dependencies.
    /// Guild flows through Channel: Message → Channel → Guild.
    /// </summary>
    public static DiscordMessageDto ToMessageDto(this SaveMessageCommand command)
    {
        var author = new DiscordUserDto(
            command.AuthorId,
            command.AuthorUsername,
            "0",
            command.AuthorIsBot,
            command.AuthorGlobalName,
            command.AuthorAvatarUrl);

        DiscordGuildDto? guild = command.GuildId.HasValue && command.GuildName is not null
            ? new DiscordGuildDto(command.GuildId.Value, command.GuildName)
            : null;

        var channel = new DiscordChannelDto(command.ChannelId, command.ChannelName, guild);

        return new DiscordMessageDto(
            DiscordId: command.MessageId,
            Content: command.Content,
            Timestamp: command.Timestamp,
            Author: author,
            Channel: channel);
    }

    /// <summary>
    /// Maps a SaveGuildCommand to a DiscordGuildDto.
    /// </summary>
    [MapProperty(nameof(SaveGuildCommand.GuildId), nameof(DiscordGuildDto.DiscordId))]
    public static partial DiscordGuildDto ToGuildDto(this SaveGuildCommand command);

    public static DiscordChannelDto ToChannelDto(this SaveChannelCommand command)
    {
        DiscordGuildDto? guild = command.GuildId.HasValue && command.GuildName is not null
            ? new DiscordGuildDto(command.GuildId.Value, command.GuildName)
            : null;

        return new DiscordChannelDto(command.ChannelId, command.Name, guild);
    }

    public static DiscordUserDto ToUserDto(this SaveUserCommand command)
        => new(
            command.UserId,
            command.Username,
            command.Discriminator,
            command.IsBot,
            command.GlobalName,
            command.AvatarUrl);

    // Role mappings
    public static DiscordRoleDto ToRoleDto(this SaveRoleCommand command)
        => new(
            DiscordId: command.RoleId,
            Name: command.Name,
            Color: command.Color,
            IsHoisted: command.IsHoisted,
            Position: command.Position,
            Permissions: (DiscordPermissions)command.Permissions,
            IsManaged: command.IsManaged,
            IsMentionable: command.IsMentionable,
            IconUrl: command.IconUrl,
            UnicodeEmoji: command.UnicodeEmoji,
            Tags: command.Tags);

    // Invite mappings
    public static DiscordInviteDto ToInviteDto(this SaveInviteCommand command)
        => new(
            DiscordId: command.InviteId,
            Code: command.Code,
            GuildId: command.GuildId,
            ChannelId: command.ChannelId,
            InviterDiscordId: command.InviterId,
            MaxAge: command.MaxAge,
            MaxUses: command.MaxUses,
            Uses: command.Uses,
            IsTemporary: command.IsTemporary,
            ExpiresAt: command.ExpiresAt,
            TargetType: command.TargetType,
            TargetUserId: command.TargetUserId,
            TargetApplicationId: command.TargetApplicationId);

    // Reaction mappings
    public static DiscordReactionDto ToReactionDto(this SaveReactionCommand command)
        => new(
            EmojiName: command.EmojiName,
            MessageDiscordId: command.MessageId,
            UserDiscordId: command.UserId,
            EmojiId: command.EmojiId,
            IsCustomEmoji: command.IsCustomEmoji,
            IsAnimated: command.IsAnimated,
            IsBurst: command.IsBurst);

    // Voice session mappings
    public static DiscordVoiceSessionDto ToVoiceSessionDto(this SaveVoiceSessionCommand command)
        => new(
            SessionId: command.SessionId,
            UserDiscordId: command.UserId,
            ChannelDiscordId: command.ChannelId,
            GuildDiscordId: command.GuildId,
            JoinedAt: command.JoinedAt,
            IsSelfMuted: command.IsSelfMuted,
            IsSelfDeafened: command.IsSelfDeafened,
            IsServerMuted: command.IsServerMuted,
            IsServerDeafened: command.IsServerDeafened,
            IsStreaming: command.IsStreaming,
            IsVideoEnabled: command.IsVideoEnabled,
            IsSuppressed: command.IsSuppressed,
            RequestToSpeakTimestamp: command.RequestToSpeakTimestamp);

    // Presence log mappings
    public static DiscordPresenceLogDto ToPresenceLogDto(this SavePresenceLogCommand command)
        => new(
            UserDiscordId: command.UserId,
            Status: command.Status,
            Timestamp: command.Timestamp,
            GuildDiscordId: command.GuildId,
            PreviousStatus: command.PreviousStatus,
            ClientStatus: command.ClientStatus,
            ActivityType: command.ActivityType,
            ActivityName: command.ActivityName,
            ActivityDetails: command.ActivityDetails,
            ActivityState: command.ActivityState,
            ActivityUrl: command.ActivityUrl,
            CustomStatusEmoji: command.CustomStatusEmoji);

    // User ban mappings
    public static DiscordUserBanDto ToBanDto(this SaveBanCommand command)
        => new(
            UserDiscordId: command.UserId,
            GuildDiscordId: command.GuildId,
            BannedAt: command.BannedAt,
            Reason: command.Reason,
            BannedByDiscordId: command.BannedById);

    // Scheduled event mappings
    public static DiscordScheduledEventDto ToScheduledEventDto(this SaveScheduledEventCommand command)
        => new(
            DiscordId: command.EventId,
            Name: command.Name,
            GuildDiscordId: command.GuildId,
            ScheduledStartTime: command.ScheduledStartTime,
            PrivacyLevel: command.PrivacyLevel,
            Status: command.Status,
            EntityType: command.EntityType,
            Description: command.Description,
            ScheduledEndTime: command.ScheduledEndTime,
            EntityMetadata: command.EntityMetadata,
            UserCount: command.UserCount,
            CoverImageUrl: command.CoverImageUrl,
            ChannelDiscordId: command.ChannelId,
            CreatorDiscordId: command.CreatorId);

    // Nickname mappings
    public static DiscordUserNicknameDto ToNicknameDto(this SaveNicknameCommand command)
        => new(
            UserDiscordId: command.UserId,
            GuildDiscordId: command.GuildId,
            Nickname: command.Nickname,
            PreviousNickname: command.PreviousNickname,
            ChangedByDiscordId: command.ChangedById);

    // Attachment mappings
    public static DiscordAttachmentDto ToAttachmentDto(this SaveAttachmentCommand command)
        => new(
            DiscordId: command.AttachmentId,
            MessageDiscordId: command.MessageId,
            Filename: command.Filename,
            Url: command.Url,
            Size: command.Size,
            ProxyUrl: command.ProxyUrl,
            Width: command.Width,
            Height: command.Height,
            ContentType: command.ContentType,
            IsSpoiler: command.IsSpoiler,
            IsVoiceMessage: command.IsVoiceMessage,
            DurationSecs: command.DurationSecs,
            Waveform: command.Waveform);

    // Sticker mappings
    public static DiscordStickerDto ToStickerDto(this SaveStickerCommand command)
        => new(
            DiscordId: command.StickerId,
            Name: command.Name,
            FormatType: command.FormatType,
            IsAvailable: command.IsAvailable,
            Description: command.Description,
            Tags: command.Tags,
            PackId: command.PackId,
            SortValue: command.SortValue,
            GuildDiscordId: command.GuildId,
            CreatorDiscordId: command.CreatorId);

    // Audit log mappings
    public static DiscordAuditLogDto ToAuditLogDto(this SaveAuditLogCommand command)
        => new(
            EntityType: command.EntityType,
            EntityId: command.EntityId,
            EntityDiscordId: command.EntityDiscordId,
            Action: command.Action,
            Timestamp: command.Timestamp,
            ChangeType: command.ChangeType,
            OldValue: command.OldValue,
            NewValue: command.NewValue,
            Reason: command.Reason,
            GuildDiscordId: command.GuildId,
            PerformedByDiscordId: command.PerformedById);
}
