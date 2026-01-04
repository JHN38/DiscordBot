using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Service.RecordKeeping.Persistence.Data;

public interface IAppDbContext
{
    // Existing DbSets
    DbSet<DiscordChannel> Channels { get; }
    DbSet<DiscordGuild> Guilds { get; }
    DbSet<DiscordMessage> Messages { get; }
    DbSet<DiscordUser> Users { get; }

    // New DbSets
    DbSet<DiscordRole> Roles { get; }
    DbSet<DiscordInvite> Invites { get; }
    DbSet<DiscordReaction> Reactions { get; }
    DbSet<DiscordVoiceSession> VoiceSessions { get; }
    DbSet<DiscordPresenceLog> PresenceLogs { get; }
    DbSet<DiscordUserBan> UserBans { get; }
    DbSet<DiscordScheduledEvent> ScheduledEvents { get; }
    DbSet<DiscordAttachment> Attachments { get; }
    DbSet<DiscordEmbed> Embeds { get; }
    DbSet<DiscordSticker> Stickers { get; }
    DbSet<DiscordAuditLog> AuditLogs { get; }
    DbSet<DiscordUserNickname> UserNicknames { get; }
    DbSet<DiscordGuildUserRole> GuildUserRoles { get; }
    DbSet<DiscordMessageSticker> MessageStickers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
