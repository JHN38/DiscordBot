namespace DiscordBot.Domain.WebSearch.Models;

public record WebSearchResponse(string Title, string Link, string Snippet, string? ThumbnailUrl);