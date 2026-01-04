namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord user nickname history.
/// </summary>
public sealed record DiscordUserNicknameDto(
    ulong UserDiscordId,
    ulong GuildDiscordId,
    string? Nickname = null,
    string? PreviousNickname = null,
    ulong? ChangedByDiscordId = null);
