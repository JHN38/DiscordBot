using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Service.RecordKeeping.Core.Interfaces;

/// <summary>
/// Manages Discord entity persistence with GetOrAdd semantics.
/// All operations use a single DbContext per call for consistency.
/// </summary>
public interface IDiscordEntityManager
{
    /// <summary>
    /// Gets or creates a message by Discord ID, ensuring all dependencies exist.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddMessageAsync(DiscordMessageDto message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a guild by Discord ID.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddGuildAsync(DiscordGuildDto guild, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a user by Discord ID.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddUserAsync(DiscordUserDto user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a channel by Discord ID, optionally with guild association.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddChannelAsync(DiscordChannelDto channel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a role by Discord ID, ensuring guild dependency exists.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddRoleAsync(DiscordRoleDto role, ulong guildDiscordId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates an invite by Discord ID, ensuring guild and channel dependencies exist.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddInviteAsync(DiscordInviteDto invite, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a reaction, ensuring message and user dependencies exist.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddReactionAsync(DiscordReactionDto reaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a voice session, ensuring user, channel, and guild dependencies exist.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddVoiceSessionAsync(DiscordVoiceSessionDto voiceSession, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a presence log entry, ensuring user and optional guild dependencies exist.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddPresenceLogAsync(DiscordPresenceLogDto presenceLog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a user ban record, ensuring user and guild dependencies exist.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddUserBanAsync(DiscordUserBanDto userBan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a scheduled event by Discord ID, ensuring guild and optional channel/creator dependencies exist.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddScheduledEventAsync(DiscordScheduledEventDto scheduledEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates an attachment by Discord ID, ensuring message dependency exists.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddAttachmentAsync(DiscordAttachmentDto attachment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates an embed, ensuring message dependency exists.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddEmbedAsync(DiscordEmbedDto embed, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a sticker by Discord ID, ensuring optional guild and creator dependencies exist.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddStickerAsync(DiscordStickerDto sticker, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a user nickname record, ensuring user and guild dependencies exist.
    /// </summary>
    Task<EntityReferenceDto> GetOrAddUserNicknameAsync(DiscordUserNicknameDto nickname, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an audit log entry, ensuring optional guild and performer dependencies exist.
    /// </summary>
    Task<EntityReferenceDto> AddAuditLogAsync(DiscordAuditLogDto auditLog, CancellationToken cancellationToken = default);
}
