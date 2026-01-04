using DiscordBot.Service.RecordKeeping.Core.Domain.Common;
using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a Discord guild role.
/// </summary>
public class DiscordRole : DiscordEntity
{
    public required string Name { get; set; }
    public int Color { get; set; } // 0 = no color
    public bool IsHoisted { get; set; } // Displayed separately in member list
    public int Position { get; set; } // Role position (hierarchy)
    public DiscordPermissions Permissions { get; set; }
    public bool IsManaged { get; set; } // Managed by integration/bot
    public bool IsMentionable { get; set; } // Can be mentioned
    public string? IconUrl { get; set; }
    public string? UnicodeEmoji { get; set; }
    public string? Tags { get; set; } // JSON role tags
    public bool IsDeleted { get; set; } // Soft delete flag
    public DateTimeOffset? DeletedAt { get; set; }

    // Navigation properties
    public int GuildId { get; set; }
    public virtual DiscordGuild Guild { get; set; } = default!;

    // Many-to-many with users via GuildUserRole
    public virtual ICollection<DiscordUser> Users { get; } = [];
}
