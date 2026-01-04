namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord audit log entries.
/// </summary>
public sealed record DiscordAuditLogDto(
    string EntityType,
    int EntityId,
    ulong EntityDiscordId,
    string Action,
    DateTimeOffset Timestamp,
    string? ChangeType = null,
    string? OldValue = null,
    string? NewValue = null,
    string? Reason = null,
    ulong? GuildDiscordId = null,
    ulong? PerformedByDiscordId = null);
