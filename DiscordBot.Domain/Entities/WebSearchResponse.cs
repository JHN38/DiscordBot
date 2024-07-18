using System.Collections.Immutable;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents a collection of search result items.
/// </summary>
public record WebSearchResponse(
    ImmutableList<WebSearchResponseItem> Items);

/// <summary>
/// Represents a common base model for search result items.
/// </summary>
public record WebSearchResponseItem(
    string Title,
    string Link,
    string DisplayLink,
    string Snippet,
    List<WebSearchImage> MainImages,
    List<WebSearchImage> Thumbnails);

/// <summary>
/// Represents a common base model for web search images.
/// </summary>
public record WebSearchImage(
    string Src,
    string Width,
    string Height);