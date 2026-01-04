using DiscordBot.Service.RecordKeeping.Core.Domain.Common;
using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Represents a Discord sticker.
/// </summary>
public class DiscordSticker : DiscordEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public StickerFormatType FormatType { get; set; }
    public string? Tags { get; set; } // Autocomplete tags
    public ulong? PackId { get; set; } // Sticker pack ID
    public int? SortValue { get; set; } // Sort position
    public bool IsAvailable { get; set; } // Available for use

    // Navigation properties
    public int? GuildId { get; set; } // Null for standard stickers
    public virtual DiscordGuild? Guild { get; set; }

    public int? UserId { get; set; } // Creator
    public virtual DiscordUser? User { get; set; }

    // Many-to-many with messages via MessageStickers
    public virtual ICollection<DiscordMessage> Messages { get; } = [];
}
