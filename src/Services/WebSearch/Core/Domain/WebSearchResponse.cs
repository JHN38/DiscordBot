namespace DiscordBot.Service.WebSearch.Core.Domain;

public record WebSearchResponse(
    IEnumerable<WebSearchResponseItem> Items);

public record WebSearchResponseItem(
    string Title,
    Uri Link,
    string DisplayLink,
    string Snippet,
    IEnumerable<WebSearchImage> Thumbnails);

public record WebSearchImage(
    Uri Src,
    string Width,
    string Height);
