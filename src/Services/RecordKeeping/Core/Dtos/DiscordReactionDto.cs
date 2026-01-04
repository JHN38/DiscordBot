namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord reaction information.
/// </summary>
public sealed record DiscordReactionDto(
    ulong MessageDiscordId,
    ulong UserDiscordId,
    string EmojiName,
    ulong? EmojiId = null,
    bool IsCustomEmoji = false,
    bool IsAnimated = false,
    bool IsBurst = false);
