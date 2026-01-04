namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord scheduled event information.
/// </summary>
public sealed record DiscordScheduledEventDto(
    ulong DiscordId,
    string Name,
    ulong GuildDiscordId,
    DateTimeOffset ScheduledStartTime,
    int PrivacyLevel,
    int Status,
    int EntityType,
    string? Description = null,
    DateTimeOffset? ScheduledEndTime = null,
    string? EntityMetadata = null,
    int? UserCount = null,
    string? CoverImageUrl = null,
    ulong? ChannelDiscordId = null,
    ulong? CreatorDiscordId = null);
