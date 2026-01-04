namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord guild information.
/// Used for passing guild data through the GetOrAdd flow without coupling to entities.
/// </summary>
public sealed record DiscordGuildDto(
    ulong DiscordId,
    string Name);
