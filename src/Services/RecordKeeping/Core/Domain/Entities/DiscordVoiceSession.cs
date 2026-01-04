using DiscordBot.Service.RecordKeeping.Core.Domain.Common;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a voice channel session for a user.
/// </summary>
public class DiscordVoiceSession : AuditableEntityBase<int>
{
    public required string SessionId { get; set; } // Discord voice session ID
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? LeftAt { get; set; }
    public bool IsSelfMuted { get; set; }
    public bool IsSelfDeafened { get; set; }
    public bool IsServerMuted { get; set; }
    public bool IsServerDeafened { get; set; }
    public bool IsStreaming { get; set; } // Screen sharing
    public bool IsVideoEnabled { get; set; } // Camera enabled
    public bool IsSuppressed { get; set; } // Suppressed in stage
    public DateTimeOffset? RequestToSpeakTimestamp { get; set; } // Stage speak request

    // Navigation properties
    public int UserId { get; set; }
    public virtual DiscordUser User { get; set; } = default!;

    public int ChannelId { get; set; }
    public virtual DiscordChannel Channel { get; set; } = default!;

    public int GuildId { get; set; }
    public virtual DiscordGuild Guild { get; set; } = default!;
}
