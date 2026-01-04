using DiscordBot.Service.RecordKeeping.Core.Domain.Common;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

public class DiscordUser : DiscordEntity
{
    public required string Username { get; set; }
    public required string Discriminator { get; set; }

    // New properties from schema expansion
    public string? GlobalName { get; set; } // New display name system
    public string? AvatarUrl { get; set; }
    public string? BannerUrl { get; set; }
    public int? AccentColor { get; set; }
    public bool IsBot { get; set; }
    public bool IsSystem { get; set; }
    public bool IsWebhook { get; set; }
    public int PublicFlags { get; set; } // User badge flags
    public int? PremiumType { get; set; } // Nitro subscription type
    public string? Locale { get; set; }

    // Existing collections
    public virtual ICollection<DiscordMessage> Messages { get; } = [];
    public virtual ICollection<DiscordChannel> Channels { get; } = [];
    public virtual ICollection<DiscordGuild> Guilds { get; } = [];

    // New collections from schema expansion
    public virtual ICollection<DiscordReaction> Reactions { get; } = [];
    public virtual ICollection<DiscordVoiceSession> VoiceSessions { get; } = [];
    public virtual ICollection<DiscordPresenceLog> PresenceLogs { get; } = [];
    public virtual ICollection<DiscordUserBan> Bans { get; } = []; // Bans received
    public virtual ICollection<DiscordUserBan> BansIssued { get; } = []; // Bans issued as moderator
    public virtual ICollection<DiscordUserNickname> Nicknames { get; } = [];
    public virtual ICollection<DiscordRole> Roles { get; } = []; // Via GuildUserRoles
    public virtual ICollection<DiscordInvite> InvitesCreated { get; } = [];
    public virtual ICollection<DiscordScheduledEvent> ScheduledEventsCreated { get; } = [];
    public virtual ICollection<DiscordSticker> StickersCreated { get; } = [];
    public virtual ICollection<DiscordAuditLog> AuditLogsPerformed { get; } = [];
}
