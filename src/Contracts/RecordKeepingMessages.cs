using System;

namespace DiscordBot.Contracts.RecordKeeping;

// Existing commands
public record SaveMessageCommand(
    ulong MessageId,
    ulong ChannelId,
    string ChannelName,
    ulong? GuildId,
    string? GuildName,
    ulong AuthorId,
    string AuthorUsername,
    string Content,
    DateTimeOffset Timestamp,
    bool AuthorIsBot = false,
    string? AuthorGlobalName = null,
    string? AuthorAvatarUrl = null);
public record SaveGuildCommand(ulong GuildId, string Name);
public record SaveChannelCommand(ulong ChannelId, string Name, ulong? GuildId, string? GuildName);
public record SaveUserCommand(
    ulong UserId,
    string Username,
    string Discriminator,
    bool IsBot = false,
    string? GlobalName = null,
    string? AvatarUrl = null);
public record LogEventCommand(string Level, string Message, string? Exception);

// Message-related commands
public record UpdateMessageCommand(ulong MessageId, string Content, DateTimeOffset EditedTimestamp);
public record DeleteMessageCommand(ulong MessageId, DateTimeOffset DeletedAt);
public record BulkDeleteMessagesCommand(ulong[] MessageIds, DateTimeOffset DeletedAt);

// Role commands
public record SaveRoleCommand(
    ulong RoleId,
    ulong GuildId,
    string Name,
    int Color,
    bool IsHoisted,
    int Position,
    ulong Permissions,
    bool IsManaged,
    bool IsMentionable,
    string? IconUrl = null,
    string? UnicodeEmoji = null,
    string? Tags = null);
public record DeleteRoleCommand(ulong RoleId, DateTimeOffset DeletedAt);

// Ban commands
public record SaveBanCommand(
    ulong UserId,
    ulong GuildId,
    DateTimeOffset BannedAt,
    string? Reason = null,
    ulong? BannedById = null);
public record UpdateBanCommand(
    ulong UserId,
    ulong GuildId,
    DateTimeOffset UnbannedAt,
    ulong? UnbannedById = null);

// Invite commands
public record SaveInviteCommand(
    ulong InviteId,
    string Code,
    ulong GuildId,
    ulong ChannelId,
    ulong? InviterId = null,
    int? MaxAge = null,
    int? MaxUses = null,
    int Uses = 0,
    bool IsTemporary = false,
    DateTimeOffset? ExpiresAt = null,
    int? TargetType = null,
    ulong? TargetUserId = null,
    ulong? TargetApplicationId = null);
public record DeleteInviteCommand(string Code, DateTimeOffset RevokedAt);

// Scheduled event commands
public record SaveScheduledEventCommand(
    ulong EventId,
    string Name,
    ulong GuildId,
    DateTimeOffset ScheduledStartTime,
    int PrivacyLevel,
    int Status,
    int EntityType,
    string? Description = null,
    DateTimeOffset? ScheduledEndTime = null,
    string? EntityMetadata = null,
    int? UserCount = null,
    string? CoverImageUrl = null,
    ulong? ChannelId = null,
    ulong? CreatorId = null);

// Voice session commands
public record SaveVoiceSessionCommand(
    string SessionId,
    ulong UserId,
    ulong ChannelId,
    ulong GuildId,
    DateTimeOffset JoinedAt,
    bool IsSelfMuted = false,
    bool IsSelfDeafened = false,
    bool IsServerMuted = false,
    bool IsServerDeafened = false,
    bool IsStreaming = false,
    bool IsVideoEnabled = false,
    bool IsSuppressed = false,
    DateTimeOffset? RequestToSpeakTimestamp = null);
public record UpdateVoiceSessionCommand(
    string SessionId,
    DateTimeOffset LeftAt);

// Presence log command
public record SavePresenceLogCommand(
    ulong UserId,
    int Status,
    DateTimeOffset Timestamp,
    ulong? GuildId = null,
    int? PreviousStatus = null,
    string? ClientStatus = null,
    int? ActivityType = null,
    string? ActivityName = null,
    string? ActivityDetails = null,
    string? ActivityState = null,
    string? ActivityUrl = null,
    string? CustomStatusEmoji = null);

// Nickname command
public record SaveNicknameCommand(
    ulong UserId,
    ulong GuildId,
    string? Nickname,
    string? PreviousNickname = null,
    ulong? ChangedById = null);

// Reaction commands
public record SaveReactionCommand(
    ulong MessageId,
    ulong UserId,
    string EmojiName,
    ulong? EmojiId = null,
    bool IsCustomEmoji = false,
    bool IsAnimated = false,
    bool IsBurst = false);
public record RemoveReactionCommand(
    ulong MessageId,
    ulong UserId,
    string EmojiName,
    ulong? EmojiId = null,
    DateTimeOffset RemovedAt = default);
public record ClearReactionsCommand(ulong MessageId, DateTimeOffset RemovedAt);
public record RemoveEmoteReactionsCommand(
    ulong MessageId,
    string EmojiName,
    ulong? EmojiId = null,
    DateTimeOffset RemovedAt = default);

// Attachment command
public record SaveAttachmentCommand(
    ulong AttachmentId,
    ulong MessageId,
    string Filename,
    string Url,
    long Size,
    string? ProxyUrl = null,
    int? Width = null,
    int? Height = null,
    string? ContentType = null,
    bool IsSpoiler = false,
    bool IsVoiceMessage = false,
    float? DurationSecs = null,
    string? Waveform = null);

// Sticker commands
public record SaveStickerCommand(
    ulong StickerId,
    string Name,
    int FormatType,
    bool IsAvailable,
    string? Description = null,
    string? Tags = null,
    ulong? PackId = null,
    int? SortValue = null,
    ulong? GuildId = null,
    ulong? CreatorId = null);

// Audit log command
public record SaveAuditLogCommand(
    string EntityType,
    int EntityId,
    ulong EntityDiscordId,
    string Action,
    DateTimeOffset Timestamp,
    string? ChangeType = null,
    string? OldValue = null,
    string? NewValue = null,
    string? Reason = null,
    ulong? GuildId = null,
    ulong? PerformedById = null);
