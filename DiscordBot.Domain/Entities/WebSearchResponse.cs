namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents a common interface for web search responses.
/// </summary>
public record WebSearchResponse(
    IEnumerable<WebSearchResponseItem> Items);

/// <summary>
/// Represents a common base model for search result items.
/// </summary>
public record WebSearchResponseItem(
    string Title,
    string Link,
    string DisplayLink,
    string Snippet,
    IEnumerable<WebSearchImage> Thumbnails);

/// <summary>
/// Represents a common base model for web search images.
/// </summary>
public record WebSearchImage(
    string Src,
    string Width,
    string Height);