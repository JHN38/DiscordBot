using DiscordBot.Service.RecordKeeping.Core.Domain.Common;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a nickname history entry for a user in a guild.
/// </summary>
public class DiscordUserNickname : AuditableEntityBase<int>
{
    public string? Nickname { get; set; } // Current/new nickname
    public string? PreviousNickname { get; set; }
    public bool IsActive { get; set; } // Currently active nickname

    // Navigation properties
    public int UserId { get; set; }
    public virtual DiscordUser User { get; set; } = default!;

    public int GuildId { get; set; }
    public virtual DiscordGuild Guild { get; set; } = default!;

    public int? ChangedById { get; set; } // User who changed the nickname
    public virtual DiscordUser? ChangedBy { get; set; }
}
