using DiscordBot.Service.RecordKeeping.Core.Domain.Common;
using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a presence status log entry for a user.
/// </summary>
public class DiscordPresenceLog : AuditableEntityBase<int>
{
    public PresenceStatus Status { get; set; }
    public PresenceStatus? PreviousStatus { get; set; }
    public string? ClientStatus { get; set; } // JSON {desktop, mobile, web}
    public ActivityType? ActivityType { get; set; }
    public string? ActivityName { get; set; }
    public string? ActivityDetails { get; set; }
    public string? ActivityState { get; set; }
    public string? ActivityUrl { get; set; } // Streaming URL
    public string? CustomStatusEmoji { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    // Navigation properties
    public int UserId { get; set; }
    public virtual DiscordUser User { get; set; } = default!;

    public int? GuildId { get; set; }
    public virtual DiscordGuild? Guild { get; set; }
}
