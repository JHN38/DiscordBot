namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord channel information.
/// Used for passing channel data through the GetOrAdd flow without coupling to entities.
/// </summary>
public sealed record DiscordChannelDto(
    ulong DiscordId,
    string Name,
    DiscordGuildDto? Guild = null);
