using DiscordBot.Service.RecordKeeping.Core.Domain.Common;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Join table linking messages to stickers.
/// </summary>
public class DiscordMessageSticker : EntityBase<int>
{
    public int MessageId { get; set; }
    public virtual DiscordMessage Message { get; set; } = default!;

    public int StickerId { get; set; }
    public virtual DiscordSticker Sticker { get; set; } = default!;
}
