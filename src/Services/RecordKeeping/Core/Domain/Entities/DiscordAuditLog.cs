using DiscordBot.Service.RecordKeeping.Core.Domain.Common;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking entity changes.
/// </summary>
public class DiscordAuditLog : EntityBase<int>
{
    public required string EntityType { get; set; } // Entity type name
    public int EntityId { get; set; } // Entity primary key
    public ulong EntityDiscordId { get; set; } // Entity Discord ID
    public required string Action { get; set; } // Created, Updated, Deleted
    public string? ChangeType { get; set; } // Specific property changed
    public string? OldValue { get; set; } // Previous value JSON
    public string? NewValue { get; set; } // New value JSON
    public string? Reason { get; set; } // Audit reason
    public DateTimeOffset Timestamp { get; set; } // Action timestamp

    // Navigation properties
    public int? GuildId { get; set; }
    public virtual DiscordGuild? Guild { get; set; }

    public int? PerformedById { get; set; }
    public virtual DiscordUser? PerformedBy { get; set; }
}
