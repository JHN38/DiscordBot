namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord user information.
/// Used for passing user data through the GetOrAdd flow without coupling to entities.
/// </summary>
public sealed record DiscordUserDto(
    ulong DiscordId,
    string Username,
    string Discriminator = "0",
    bool IsBot = false,
    string? GlobalName = null,
    string? AvatarUrl = null);
