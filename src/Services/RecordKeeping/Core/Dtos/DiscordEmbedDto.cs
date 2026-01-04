namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord embed information.
/// </summary>
public sealed record DiscordEmbedDto(
    ulong MessageDiscordId,
    string? Type = null,
    string? Title = null,
    string? Description = null,
    string? Url = null,
    DateTimeOffset? Timestamp = null,
    int? Color = null,
    string? FooterText = null,
    string? FooterIconUrl = null,
    string? ImageUrl = null,
    string? ThumbnailUrl = null,
    string? VideoUrl = null,
    string? ProviderName = null,
    string? ProviderUrl = null,
    string? AuthorName = null,
    string? AuthorUrl = null,
    string? AuthorIconUrl = null,
    string? Fields = null);
