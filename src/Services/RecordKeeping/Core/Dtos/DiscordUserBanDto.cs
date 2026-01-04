namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord user ban information.
/// </summary>
public sealed record DiscordUserBanDto(
    ulong UserDiscordId,
    ulong GuildDiscordId,
    DateTimeOffset BannedAt,
    string? Reason = null,
    ulong? BannedByDiscordId = null,
    DateTimeOffset? UnbannedAt = null,
    ulong? UnbannedByDiscordId = null,
    bool IsActive = true);
