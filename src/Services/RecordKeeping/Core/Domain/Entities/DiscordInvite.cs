using DiscordBot.Service.RecordKeeping.Core.Domain.Common;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a Discord guild invite.
/// </summary>
public class DiscordInvite : DiscordEntity
{
    public required string Code { get; set; } // Invite code (unique)
    public int? MaxAge { get; set; } // Seconds until expiry
    public int? MaxUses { get; set; }
    public int Uses { get; set; } // Current use count
    public bool IsTemporary { get; set; } // Temporary membership
    public DateTimeOffset? ExpiresAt { get; set; }
    public int? TargetType { get; set; } // Target type (Stream, Application)
    public ulong? TargetUserId { get; set; } // Target user for stream
    public ulong? TargetApplicationId { get; set; }
    public bool IsRevoked { get; set; } // Revoked/deleted flag
    public DateTimeOffset? RevokedAt { get; set; }

    // Navigation properties
    public int GuildId { get; set; }
    public virtual DiscordGuild Guild { get; set; } = default!;

    public int ChannelId { get; set; }
    public virtual DiscordChannel Channel { get; set; } = default!;

    public int? InviterId { get; set; } // Creator
    public virtual DiscordUser? Inviter { get; set; }
}
