using System.Collections.Immutable;
using System.Text.Json.Serialization;
using DiscordBot.Service.WebSearch.Core.Domain;

namespace DiscordBot.Service.WebSearch.Infrastructure.Services.SerpApi;

public record SerpApiWebSearchResponse(
    [property: JsonPropertyName("items")] ImmutableList<SerpApiWebSearchResponseItem> Items)
{
    public WebSearchResponse ToWebSearchResult() =>
        new(
            Items.ConvertAll(SerpApiItem => new WebSearchResponseItem(
                SerpApiItem.Title,
                new Uri(SerpApiItem.Link),
                SerpApiItem.DisplayLink,
                SerpApiItem.Snippet,
                SerpApiItem.Pagemap?.Thumbnails?.Select(img => img.ToWebSearchImage()).ToList() ?? [])));
}

public record SerpApiWebSearchResponseItem(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("link")] string Link,
    [property: JsonPropertyName("displayLink")] string DisplayLink,
    [property: JsonPropertyName("snippet")] string Snippet,
    [property: JsonPropertyName("pagemap")] SerpApiWebSearchPageMap Pagemap);

public record SerpApiWebSearchPageMap(
    [property: JsonPropertyName("cse_image")] IReadOnlyList<SerpApiWebSearchImage>? MainImages,
    [property: JsonPropertyName("cse_thumbnail")] IReadOnlyList<SerpApiWebSearchImage>? Thumbnails);

public record SerpApiWebSearchImage(
    [property: JsonPropertyName("src")] string Src,
    [property: JsonPropertyName("width")] string Width,
    [property: JsonPropertyName("height")] string Height)
{
    public WebSearchImage ToWebSearchImage() => new(new Uri(Src), Width, Height);
}
