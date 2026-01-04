namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord presence log information.
/// </summary>
public sealed record DiscordPresenceLogDto(
    ulong UserDiscordId,
    int Status,
    DateTimeOffset Timestamp,
    ulong? GuildDiscordId = null,
    int? PreviousStatus = null,
    string? ClientStatus = null,
    int? ActivityType = null,
    string? ActivityName = null,
    string? ActivityDetails = null,
    string? ActivityState = null,
    string? ActivityUrl = null,
    string? CustomStatusEmoji = null);
