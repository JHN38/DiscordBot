using DiscordBot.Service.RecordKeeping.Core.Domain.Common;
using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Service.RecordKeeping.Persistence.Services;

/// <summary>
/// Manages Discord entity persistence with GetOrAdd semantics.
/// Dependencies flow logically: Message → (Author, Channel) → Guild.
/// Uses parallel resolution for independent dependencies.
///
/// Dependency handling policy:
/// - When a method receives a full DTO (e.g., DiscordGuildDto), the entity will be created/updated from that DTO.
/// - When a method only receives a Discord ID for a dependency, the dependency MUST already exist.
/// - Methods throw InvalidOperationException if a required dependency doesn't exist.
/// </summary>
public sealed partial class DiscordEntityManager(
    IDbContextFactory<AppDbContext> factory,
    ILogger<DiscordEntityManager> logger) : IDiscordEntityManager
{
    private const string REFERENCED_MESSAGE_ID_PROPERTY = "ReferencedMessageId";
    private const ulong NO_DISCORD_ID = 0;

    /// <inheritdoc />
    /// <remarks>
    /// This method intentionally uses two separate DbContext instances:
    /// 1. First context (checkDb): Read-only existence check with AsNoTracking
    /// 2. ResolveDependenciesAsync: Creates additional contexts for parallel dependency resolution
    /// 3. Second context (db): Fresh context for entity materialization and save
    ///
    /// This separation prevents entity tracking conflicts when ResolveAsync creates
    /// entities in parallel contexts that would otherwise conflict with the main
    /// save operation's change tracker.
    /// </remarks>
    public async Task<EntityReferenceDto> GetOrAddMessageAsync(DiscordMessageDto message, CancellationToken cancellationToken = default)
    {
        // Check if message already exists
        await using var checkDb = await factory.CreateDbContextAsync(cancellationToken);
        var existing = await checkDb.Messages
            .AsNoTracking()
            .Where(m => m.DiscordId == message.DiscordId)
            .Select(m => new { m.Id, m.DiscordId })
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is not null)
            return new EntityReferenceDto(existing.Id, existing.DiscordId);

        LogSavingMessage(message.DiscordId, message.Author.DiscordId, message.Channel.DiscordId);

        // Resolve dependencies in parallel: Author and Channel (Channel includes Guild)
        var resolutions = await ResolveDependenciesAsync(message, cancellationToken);

        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Materialize in dependency order: Guild → Channel → Author
        var guild = MaterializeOptional(db, resolutions.Channel.Guild);
        var channel = Materialize(db, resolutions.Channel.Channel);
        var author = Materialize(db, resolutions.Author);

        // Link new channel to guild
        if (guild is not null && !resolutions.Channel.Channel.Exists)
            channel.Guild = guild;

        // Create message with dependencies (Guild derived from Channel)
        var messageEntity = message.ToEntity();
        messageEntity.Author = author;
        messageEntity.Channel = channel;
        messageEntity.Guild = guild;

        if (message.ReferencedMessageDiscordId is { } refId)
            await LinkReferencedMessageAsync(db, messageEntity, refId, cancellationToken);

        db.Messages.Add(messageEntity);
        await db.SaveChangesAsync(cancellationToken);

        LogMessageSaved(message.DiscordId, messageEntity.Id);
        return new EntityReferenceDto(messageEntity.Id, messageEntity.DiscordId);
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddGuildAsync(DiscordGuildDto guild, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);
        var entity = await db.Guilds.GetOrCreateByDiscordIdAsync(db, guild.DiscordId, guild.ToEntity, cancellationToken);
        return new EntityReferenceDto(entity.Id, entity.DiscordId);
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddUserAsync(DiscordUserDto user, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);
        var entity = await db.Users.GetOrCreateByDiscordIdAsync(db, user.DiscordId, user.ToEntity, cancellationToken);
        return new EntityReferenceDto(entity.Id, entity.DiscordId);
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddChannelAsync(DiscordChannelDto channel, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        DiscordGuild? guildEntity = null;
        if (channel.Guild is { } g)
            guildEntity = await db.Guilds.GetOrCreateByDiscordIdAsync(db, g.DiscordId, g.ToEntity, cancellationToken);

        var entity = await db.Channels.GetOrCreateByDiscordIdAsync(db, channel.DiscordId, CreateChannel, cancellationToken);
        return new EntityReferenceDto(entity.Id, entity.DiscordId);

        DiscordChannel CreateChannel()
        {
            var ch = channel.ToEntity();
            ch.Guild = guildEntity;
            return ch;
        }
    }

    private async Task<MessageDependencies> ResolveDependenciesAsync(DiscordMessageDto message, CancellationToken ct)
    {
        // Author and Channel are independent - resolve in parallel
        var authorTask = ResolveAsync(message.Author.DiscordId, message.Author.ToEntity, ct);
        var channelTask = ResolveChannelAsync(message.Channel, ct);

        await Task.WhenAll(authorTask, channelTask);

        return new MessageDependencies(await authorTask, await channelTask);
    }

    private async Task<ChannelResolution> ResolveChannelAsync(DiscordChannelDto channel, CancellationToken ct)
    {
        // Resolve channel and its guild dependency (struct implicitly converts to nullable)
        var guildTask = channel.Guild is { } g
            ? ResolveAsync(g.DiscordId, g.ToEntity, ct).ContinueWith(t => (EntityResolution<DiscordGuild>?)t.Result, ct, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default)
            : Task.FromResult<EntityResolution<DiscordGuild>?>(null);

        var channelTask = ResolveAsync(channel.DiscordId, channel.ToEntity, ct);

        await Task.WhenAll(guildTask, channelTask);

        return new ChannelResolution(await channelTask, await guildTask);
    }

    private async Task<EntityResolution<TEntity>> ResolveAsync<TEntity>(
        ulong discordId,
        Func<TEntity> createEntity,
        CancellationToken ct)
        where TEntity : DiscordEntity
    {
        await using var db = await factory.CreateDbContextAsync(ct);

        var existingId = await db.Set<TEntity>()
            .AsNoTracking()
            .Where(e => e.DiscordId == discordId)
            .Select(e => (int?)e.Id)
            .FirstOrDefaultAsync(ct);

        return new EntityResolution<TEntity>(existingId, discordId, createEntity);
    }


    private async Task LinkReferencedMessageAsync(
        AppDbContext db,
        DiscordMessage messageEntity,
        ulong referencedDiscordId,
        CancellationToken ct)
    {
        var refMessageId = await db.Messages
            .Where(m => m.DiscordId == referencedDiscordId)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync(ct);

        if (refMessageId is { } id)
        {
            db.Entry(messageEntity).Property(REFERENCED_MESSAGE_ID_PROPERTY).CurrentValue = id;
            LogLinkedReferencedMessage(id);
        }
        else
        {
            LogReferencedMessageNotFound(referencedDiscordId);
        }
    }

    private static TEntity Materialize<TEntity>(AppDbContext db, EntityResolution<TEntity> resolution)
        where TEntity : DiscordEntity
    {
        var entity = resolution.CreateNew();

        if (resolution.Exists)
        {
            db.Entry(entity).Property(e => e.Id).CurrentValue = resolution.ExistingId!.Value;
            db.Entry(entity).State = EntityState.Unchanged;
        }
        else
        {
            db.Set<TEntity>().Add(entity);
        }

        return entity;
    }

    private static TEntity? MaterializeOptional<TEntity>(AppDbContext db, EntityResolution<TEntity>? resolution)
        where TEntity : DiscordEntity
        => resolution is { } r ? Materialize(db, r) : null;

    /// <summary>
    /// Gets an existing entity by Discord ID or throws if not found.
    /// </summary>
    private static async Task<TEntity> GetExistingAsync<TEntity>(
        AppDbContext db,
        ulong discordId,
        CancellationToken ct)
        where TEntity : DiscordEntity
    {
        var entity = await db.Set<TEntity>()
            .Where(e => e.DiscordId == discordId)
            .FirstOrDefaultAsync(ct);

        return entity ?? throw new InvalidOperationException(
            $"{typeof(TEntity).Name} with Discord ID {discordId} not found. " +
            $"Ensure the {typeof(TEntity).Name.ToLowerInvariant()} is created before referencing it.");
    }

    /// <summary>
    /// Gets an existing entity by Discord ID or returns null if not found (for optional dependencies).
    /// </summary>
    private static async Task<TEntity?> GetOptionalAsync<TEntity>(
        AppDbContext db,
        ulong? discordId,
        CancellationToken ct)
        where TEntity : DiscordEntity
    {
        if (!discordId.HasValue)
            return null;

        return await db.Set<TEntity>()
            .Where(e => e.DiscordId == discordId.Value)
            .FirstOrDefaultAsync(ct);
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddRoleAsync(DiscordRoleDto role, ulong guildDiscordId, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Guild must already exist - throw if not found
        var guild = await GetExistingAsync<DiscordGuild>(db, guildDiscordId, cancellationToken);

        // Get or create the role
        var entity = await db.Roles.GetOrCreateByDiscordIdAsync(db, role.DiscordId, CreateRole, cancellationToken);
        return new EntityReferenceDto(entity.Id, entity.DiscordId);

        DiscordRole CreateRole()
        {
            var r = role.ToEntity();
            r.Guild = guild;
            return r;
        }
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddInviteAsync(DiscordInviteDto invite, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Dependencies must already exist - throw if not found
        var guild = await GetExistingAsync<DiscordGuild>(db, invite.GuildId, cancellationToken);
        var channel = await GetExistingAsync<DiscordChannel>(db, invite.ChannelId, cancellationToken);
        var inviter = await GetOptionalAsync<DiscordUser>(db, invite.InviterDiscordId, cancellationToken);

        // Get or create the invite
        var entity = await db.Invites.GetOrCreateByDiscordIdAsync(db, invite.DiscordId, CreateInvite, cancellationToken);
        return new EntityReferenceDto(entity.Id, entity.DiscordId);

        DiscordInvite CreateInvite()
        {
            var inv = invite.ToEntity();
            inv.Guild = guild;
            inv.Channel = channel;
            inv.Inviter = inviter;
            return inv;
        }
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddReactionAsync(DiscordReactionDto reaction, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Find message and user IDs
        var messageId = await db.Messages
            .Where(m => m.DiscordId == reaction.MessageDiscordId)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var userId = await db.Users
            .Where(u => u.DiscordId == reaction.UserDiscordId)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!messageId.HasValue || !userId.HasValue)
        {
            throw new InvalidOperationException(
                $"Cannot create reaction: Message {reaction.MessageDiscordId} or User {reaction.UserDiscordId} not found");
        }

        // Check if reaction already exists
        var existing = await db.Reactions
            .Where(r => r.MessageId == messageId.Value
                     && r.UserId == userId.Value
                     && r.EmojiName == reaction.EmojiName
                     && r.EmojiId == reaction.EmojiId)
            .Select(r => new { r.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is not null)
            return new EntityReferenceDto(existing.Id, NO_DISCORD_ID);

        // Create new reaction
        var entity = reaction.ToEntity();
        entity.MessageId = messageId.Value;
        entity.UserId = userId.Value;

        db.Reactions.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new EntityReferenceDto(entity.Id, NO_DISCORD_ID);
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddVoiceSessionAsync(DiscordVoiceSessionDto voiceSession, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Dependencies must already exist - throw if not found
        var user = await GetExistingAsync<DiscordUser>(db, voiceSession.UserDiscordId, cancellationToken);
        var channel = await GetExistingAsync<DiscordChannel>(db, voiceSession.ChannelDiscordId, cancellationToken);
        var guild = await GetExistingAsync<DiscordGuild>(db, voiceSession.GuildDiscordId, cancellationToken);

        // Check if session already exists by SessionId
        var existing = await db.VoiceSessions
            .Where(v => v.SessionId == voiceSession.SessionId)
            .Select(v => new { v.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is not null)
            return new EntityReferenceDto(existing.Id, NO_DISCORD_ID);

        // Create new voice session
        var entity = voiceSession.ToEntity();
        entity.User = user;
        entity.Channel = channel;
        entity.Guild = guild;

        db.VoiceSessions.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new EntityReferenceDto(entity.Id, NO_DISCORD_ID);
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddPresenceLogAsync(DiscordPresenceLogDto presenceLog, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // User must already exist - throw if not found
        var user = await GetExistingAsync<DiscordUser>(db, presenceLog.UserDiscordId, cancellationToken);

        // Guild is optional - get if exists
        var guild = await GetOptionalAsync<DiscordGuild>(db, presenceLog.GuildDiscordId, cancellationToken);

        // Create presence log (always a new entry)
        var entity = presenceLog.ToEntity();
        entity.User = user;
        entity.Guild = guild;

        db.PresenceLogs.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new EntityReferenceDto(entity.Id, NO_DISCORD_ID);
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddUserBanAsync(DiscordUserBanDto userBan, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Required dependencies must already exist - throw if not found
        var user = await GetExistingAsync<DiscordUser>(db, userBan.UserDiscordId, cancellationToken);
        var guild = await GetExistingAsync<DiscordGuild>(db, userBan.GuildDiscordId, cancellationToken);

        // Optional moderator dependencies - get if they exist
        var bannedBy = await GetOptionalAsync<DiscordUser>(db, userBan.BannedByDiscordId, cancellationToken);
        var unbannedBy = await GetOptionalAsync<DiscordUser>(db, userBan.UnbannedByDiscordId, cancellationToken);

        // Check if an active ban already exists
        var existing = await db.UserBans
            .Where(b => b.UserId == user.Id && b.GuildId == guild.Id && b.IsActive)
            .Select(b => new { b.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is not null)
            return new EntityReferenceDto(existing.Id, NO_DISCORD_ID);

        // Create new ban record
        var entity = userBan.ToEntity();
        entity.User = user;
        entity.Guild = guild;
        entity.BannedBy = bannedBy;
        entity.UnbannedBy = unbannedBy;

        db.UserBans.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new EntityReferenceDto(entity.Id, NO_DISCORD_ID);
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddScheduledEventAsync(DiscordScheduledEventDto scheduledEvent, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Guild must already exist - throw if not found
        var guild = await GetExistingAsync<DiscordGuild>(db, scheduledEvent.GuildDiscordId, cancellationToken);

        // Optional dependencies - get if they exist
        var channel = await GetOptionalAsync<DiscordChannel>(db, scheduledEvent.ChannelDiscordId, cancellationToken);
        var creator = await GetOptionalAsync<DiscordUser>(db, scheduledEvent.CreatorDiscordId, cancellationToken);

        // Get or create the scheduled event
        var entity = await db.ScheduledEvents.GetOrCreateByDiscordIdAsync(db, scheduledEvent.DiscordId, CreateEvent, cancellationToken);
        return new EntityReferenceDto(entity.Id, entity.DiscordId);

        DiscordScheduledEvent CreateEvent()
        {
            var evt = scheduledEvent.ToEntity();
            evt.Guild = guild;
            evt.Channel = channel;
            evt.Creator = creator;
            return evt;
        }
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddAttachmentAsync(DiscordAttachmentDto attachment, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Find message ID
        var messageId = await db.Messages
            .Where(m => m.DiscordId == attachment.MessageDiscordId)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!messageId.HasValue)
        {
            throw new InvalidOperationException(
                $"Cannot create attachment: Message {attachment.MessageDiscordId} not found");
        }

        // Get or create the attachment
        var entity = await db.Attachments.GetOrCreateByDiscordIdAsync(db, attachment.DiscordId, CreateAttachment, cancellationToken);
        return new EntityReferenceDto(entity.Id, entity.DiscordId);

        DiscordAttachment CreateAttachment()
        {
            var att = attachment.ToEntity();
            att.MessageId = messageId.Value;
            return att;
        }
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddEmbedAsync(DiscordEmbedDto embed, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Find message ID
        var messageId = await db.Messages
            .Where(m => m.DiscordId == embed.MessageDiscordId)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!messageId.HasValue)
        {
            throw new InvalidOperationException(
                $"Cannot create embed: Message {embed.MessageDiscordId} not found");
        }

        // Create embed (always a new entry, embeds don't have unique constraints)
        var entity = embed.ToEntity();
        entity.MessageId = messageId.Value;

        db.Embeds.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new EntityReferenceDto(entity.Id, NO_DISCORD_ID);
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddStickerAsync(DiscordStickerDto sticker, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Optional dependencies - get if they exist
        var guild = await GetOptionalAsync<DiscordGuild>(db, sticker.GuildDiscordId, cancellationToken);
        var creator = await GetOptionalAsync<DiscordUser>(db, sticker.CreatorDiscordId, cancellationToken);

        // Get or create the sticker
        var entity = await db.Stickers.GetOrCreateByDiscordIdAsync(db, sticker.DiscordId, CreateSticker, cancellationToken);
        return new EntityReferenceDto(entity.Id, entity.DiscordId);

        DiscordSticker CreateSticker()
        {
            var s = sticker.ToEntity();
            s.Guild = guild;
            s.User = creator;
            return s;
        }
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> GetOrAddUserNicknameAsync(DiscordUserNicknameDto nickname, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Required dependencies must already exist - throw if not found
        var user = await GetExistingAsync<DiscordUser>(db, nickname.UserDiscordId, cancellationToken);
        var guild = await GetExistingAsync<DiscordGuild>(db, nickname.GuildDiscordId, cancellationToken);

        // Optional dependency - get if exists
        var changedBy = await GetOptionalAsync<DiscordUser>(db, nickname.ChangedByDiscordId, cancellationToken);

        // Create nickname record (always a new entry for history tracking)
        var entity = nickname.ToEntity();
        entity.User = user;
        entity.Guild = guild;
        entity.ChangedBy = changedBy;

        db.UserNicknames.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new EntityReferenceDto(entity.Id, NO_DISCORD_ID);
    }

    /// <inheritdoc />
    public async Task<EntityReferenceDto> AddAuditLogAsync(DiscordAuditLogDto auditLog, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Optional dependencies - get if they exist
        var guild = await GetOptionalAsync<DiscordGuild>(db, auditLog.GuildDiscordId, cancellationToken);
        var performedBy = await GetOptionalAsync<DiscordUser>(db, auditLog.PerformedByDiscordId, cancellationToken);

        // Create audit log entry (always a new entry)
        var entity = auditLog.ToEntity();
        entity.Guild = guild;
        entity.PerformedBy = performedBy;

        db.AuditLogs.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new EntityReferenceDto(entity.Id, NO_DISCORD_ID);
    }
}

/// <summary>
/// Result of resolving a Discord entity by its Discord ID.
/// </summary>
internal readonly record struct EntityResolution<TEntity>(
    int? ExistingId,
    ulong DiscordId,
    Func<TEntity> CreateNew)
    where TEntity : DiscordEntity
{
    public bool Exists => ExistingId.HasValue;
}

/// <summary>
/// Channel with its Guild dependency.
/// </summary>
internal sealed record ChannelResolution(
    EntityResolution<DiscordChannel> Channel,
    EntityResolution<DiscordGuild>? Guild);

/// <summary>
/// All dependencies for a message: Author and Channel (Channel contains Guild).
/// </summary>
internal sealed record MessageDependencies(
    EntityResolution<DiscordUser> Author,
    ChannelResolution Channel);
