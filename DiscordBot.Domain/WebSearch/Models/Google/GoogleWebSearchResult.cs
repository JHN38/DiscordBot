using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace DiscordBot.Domain.WebSearch.Models.Google;

public record GoogleSearchResult(
    [property: JsonPropertyName("items")] ImmutableList<GoogleSearchResultItem> Items)
{
    public WebSearchResult ToWebSearchResult() =>
        new(
            Items.ConvertAll(googleItem => new WebSearchResultItem(
                googleItem.Title,
                googleItem.Link,
                googleItem.DisplayLink,
                googleItem.Snippet,
                googleItem.Pagemap?.MainImages?.ConvertAll(img => img.ToWebSearchImage()) ?? [],
                googleItem.Pagemap?.Thumbnails?.ConvertAll(img => img.ToWebSearchImage()) ?? [])));
}

public record GoogleSearchResultItem(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("link")] string Link,
    [property: JsonPropertyName("displayLink")] string DisplayLink,
    [property: JsonPropertyName("snippet")] string Snippet,
    [property: JsonPropertyName("pagemap")] GooglePageMap Pagemap);

public record GooglePageMap(
    [property: JsonPropertyName("cse_image")] List<GoogleWebSearchImage> MainImages,
    [property: JsonPropertyName("cse_thumbnail")] List<GoogleWebSearchImage> Thumbnails);

public record GoogleWebSearchImage(
    [property: JsonPropertyName("src")] string Src,
    [property: JsonPropertyName("width")] string Width,
    [property: JsonPropertyName("height")] string Height)
{
    public WebSearchImage ToWebSearchImage() => new(Src, Width, Height);
}
