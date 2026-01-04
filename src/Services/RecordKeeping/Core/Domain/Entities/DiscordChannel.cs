using DiscordBot.Service.RecordKeeping.Core.Domain.Common;
using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

public class DiscordChannel : DiscordEntity
{
    public required string Name { get; set; }

    // New properties from schema expansion
    public ChannelType ChannelType { get; set; }
    public string? Topic { get; set; }
    public int Position { get; set; }
    public bool IsNsfw { get; set; }
    public int SlowModeInterval { get; set; }
    public ulong? ParentId { get; set; } // Parent category/thread Discord ID
    public int? Bitrate { get; set; } // Voice channel bitrate
    public int? UserLimit { get; set; } // Voice channel user limit
    public int? RateLimitPerUser { get; set; }
    public string? PermissionOverwrites { get; set; } // JSON permission overwrites
    public int? DefaultAutoArchiveDuration { get; set; } // Thread auto-archive duration
    public bool? IsArchived { get; set; } // Thread archived status
    public DateTimeOffset? ArchiveTimestamp { get; set; }
    public bool? IsLocked { get; set; } // Thread locked status
    public int? DefaultThreadRateLimitPerUser { get; set; } // Forum default rate limit
    public string? AvailableTags { get; set; } // Forum tags JSON
    public string? DefaultReactionEmoji { get; set; } // Forum default reaction
    public int? DefaultSortOrder { get; set; } // Forum sort order
    public bool IsDeleted { get; set; } // Soft delete flag
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// The guild this channel belongs to. Null for DM channels.
    /// </summary>
    public virtual DiscordGuild? Guild { get; set; }

    // Self-referencing for category parent
    public int? CategoryId { get; set; }
    public virtual DiscordChannel? Category { get; set; }
    public virtual ICollection<DiscordChannel> ChildChannels { get; } = [];

    // Existing collections
    public virtual ICollection<DiscordMessage> Messages { get; } = [];
    public virtual ICollection<DiscordUser> Users { get; } = [];

    // New collections from schema expansion
    public virtual ICollection<DiscordVoiceSession> VoiceSessions { get; } = [];
    public virtual ICollection<DiscordInvite> Invites { get; } = [];
    public virtual ICollection<DiscordScheduledEvent> ScheduledEvents { get; } = [];
}
