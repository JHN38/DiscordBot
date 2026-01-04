using DiscordBot.Service.RecordKeeping.Core.Domain.Common;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a user ban record in a guild.
/// </summary>
public class DiscordUserBan : AuditableEntityBase<int>
{
    public string? Reason { get; set; }
    public DateTimeOffset BannedAt { get; set; }
    public DateTimeOffset? UnbannedAt { get; set; }
    public bool IsActive { get; set; } // Currently banned

    // Navigation properties
    public int UserId { get; set; } // Banned user
    public virtual DiscordUser User { get; set; } = default!;

    public int GuildId { get; set; }
    public virtual DiscordGuild Guild { get; set; } = default!;

    public int? BannedById { get; set; } // Moderator who banned
    public virtual DiscordUser? BannedBy { get; set; }

    public int? UnbannedById { get; set; } // Moderator who unbanned
    public virtual DiscordUser? UnbannedBy { get; set; }
}
