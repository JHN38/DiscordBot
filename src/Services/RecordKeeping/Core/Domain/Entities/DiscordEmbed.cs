using DiscordBot.Service.RecordKeeping.Core.Domain.Common;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a rich embed on a Discord message.
/// Inherits from AuditableEntityBase since embeds don't have Discord IDs but need audit timestamps.
/// </summary>
public class DiscordEmbed : AuditableEntityBase<int>
{
    public string? Type { get; set; } // Embed type (rich, image, video, etc.)
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public int? Color { get; set; }
    public string? FooterText { get; set; }
    public string? FooterIconUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? ProviderName { get; set; }
    public string? ProviderUrl { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorUrl { get; set; }
    public string? AuthorIconUrl { get; set; }
    public string? Fields { get; set; } // JSON array [{name, value, inline}]

    // Navigation properties
    public int MessageId { get; set; }
    public virtual DiscordMessage Message { get; set; } = default!;
}
