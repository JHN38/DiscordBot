using DiscordBot.Service.RecordKeeping.Core.Domain.Common;
using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

public class DiscordGuild : DiscordEntity
{
    public required string Name { get; set; }

    // New properties from schema expansion
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public string? BannerUrl { get; set; }
    public ulong OwnerId { get; set; }
    public int MemberCount { get; set; }
    public int PremiumTier { get; set; }
    public int PremiumSubscriptionCount { get; set; }
    public string? PreferredLocale { get; set; }
    public string? VanityUrlCode { get; set; }
    public bool IsNsfw { get; set; }
    public VerificationLevel VerificationLevel { get; set; }
    public DefaultMessageNotifications DefaultMessageNotifications { get; set; }
    public ExplicitContentFilter ExplicitContentFilter { get; set; }
    public ulong? SystemChannelId { get; set; }
    public ulong? RulesChannelId { get; set; }
    public ulong? PublicUpdatesChannelId { get; set; }
    public ulong? AfkChannelId { get; set; }
    public int AfkTimeout { get; set; }
    public bool WidgetEnabled { get; set; }
    public List<string> Features { get; set; } = [];
    public int? MaxMembers { get; set; }
    public int? MaxVideoChannelUsers { get; set; }
    public int? ApproximateMemberCount { get; set; }
    public int? ApproximatePresenceCount { get; set; }
    public DateTimeOffset? JoinedAt { get; set; }
    public bool IsAvailable { get; set; }

    // Existing collections
    public virtual ICollection<DiscordChannel> Channels { get; } = [];
    public virtual ICollection<DiscordUser> Users { get; } = [];
    public virtual ICollection<DiscordMessage> Messages { get; } = [];

    // New collections from schema expansion
    public virtual ICollection<DiscordRole> Roles { get; } = [];
    public virtual ICollection<DiscordInvite> Invites { get; } = [];
    public virtual ICollection<DiscordScheduledEvent> ScheduledEvents { get; } = [];
    public virtual ICollection<DiscordUserBan> Bans { get; } = [];
    public virtual ICollection<DiscordVoiceSession> VoiceSessions { get; } = [];
    public virtual ICollection<DiscordPresenceLog> PresenceLogs { get; } = [];
    public virtual ICollection<DiscordUserNickname> Nicknames { get; } = [];
    public virtual ICollection<DiscordAuditLog> AuditLogs { get; } = [];
    public virtual ICollection<DiscordSticker> Stickers { get; } = [];
}
