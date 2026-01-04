using DiscordBot.Service.RecordKeeping.Core.Domain.Common;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a reaction on a Discord message.
/// Inherits from AuditableEntityBase since reactions don't have Discord IDs.
/// </summary>
public class DiscordReaction : AuditableEntityBase<int>
{
    public required string EmojiName { get; set; } // Unicode emoji or custom name
    public ulong? EmojiId { get; set; } // Custom emoji Discord ID
    public bool IsCustomEmoji { get; set; }
    public bool IsAnimated { get; set; }
    public bool IsBurst { get; set; } // Super reaction flag
    public DateTimeOffset? RemovedAt { get; set; } // Soft delete timestamp

    // Navigation properties
    public int MessageId { get; set; }
    public virtual DiscordMessage Message { get; set; } = default!;

    public int UserId { get; set; }
    public virtual DiscordUser User { get; set; } = default!;
}
