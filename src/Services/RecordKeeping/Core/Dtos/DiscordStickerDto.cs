namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord sticker information.
/// </summary>
public sealed record DiscordStickerDto(
    ulong DiscordId,
    string Name,
    int FormatType,
    bool IsAvailable,
    string? Description = null,
    string? Tags = null,
    ulong? PackId = null,
    int? SortValue = null,
    ulong? GuildDiscordId = null,
    ulong? CreatorDiscordId = null);
