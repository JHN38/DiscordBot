using DiscordBot.Service.RecordKeeping.Core.Domain.Common;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a file attachment on a Discord message.
/// </summary>
public class DiscordAttachment : DiscordEntity
{
    public required string Filename { get; set; }
    public required string Url { get; set; }
    public string? ProxyUrl { get; set; }
    public long Size { get; set; } // File size in bytes
    public int? Width { get; set; } // Image/video width
    public int? Height { get; set; } // Image/video height
    public string? ContentType { get; set; } // MIME type
    public bool IsSpoiler { get; set; }
    public bool IsVoiceMessage { get; set; }
    public float? DurationSecs { get; set; } // Audio/video duration
    public string? Waveform { get; set; } // Voice message waveform

    // Navigation properties
    public int MessageId { get; set; }
    public virtual DiscordMessage Message { get; set; } = default!;
}
