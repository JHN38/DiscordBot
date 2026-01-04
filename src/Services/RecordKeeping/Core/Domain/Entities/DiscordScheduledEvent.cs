using DiscordBot.Service.RecordKeeping.Core.Domain.Common;
using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a Discord scheduled event in a guild.
/// </summary>
public class DiscordScheduledEvent : DiscordEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset ScheduledStartTime { get; set; }
    public DateTimeOffset? ScheduledEndTime { get; set; }
    public ScheduledEventPrivacyLevel PrivacyLevel { get; set; }
    public ScheduledEventStatus Status { get; set; }
    public ScheduledEventEntityType EntityType { get; set; }
    public string? EntityMetadata { get; set; } // JSON (location for External)
    public int? UserCount { get; set; } // Interested user count
    public string? CoverImageUrl { get; set; }

    // Navigation properties
    public int GuildId { get; set; }
    public virtual DiscordGuild Guild { get; set; } = default!;

    public int? ChannelId { get; set; }
    public virtual DiscordChannel? Channel { get; set; }

    public int? CreatorId { get; set; }
    public virtual DiscordUser? Creator { get; set; }
}
