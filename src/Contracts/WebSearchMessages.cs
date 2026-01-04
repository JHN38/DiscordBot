namespace DiscordBot.Contracts.WebSearch;

public record SearchWebQuery(string Query);

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
