using System.Collections.Immutable;
using System.Text.Json.Serialization;
using DiscordBot.Service.WebSearch.Core.Domain;

namespace DiscordBot.Service.WebSearch.Infrastructure.Services.CustomSearchEngine;

public record GoogleWebSearchResponse(
    [property: JsonPropertyName("items")] ImmutableList<GoogleWebSearchResponseItem> Items)
{
    public WebSearchResponse ToWebSearchResult() =>
        new(
            Items.ConvertAll(googleItem => new WebSearchResponseItem(
                googleItem.Title,
                new Uri(googleItem.Link),
                googleItem.DisplayLink,
                googleItem.Snippet,
                googleItem.Pagemap?.Thumbnails?.Select(img => img.ToWebSearchImage()).ToList() ?? [])));
}

public record GoogleWebSearchResponseItem(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("link")] string Link,
    [property: JsonPropertyName("displayLink")] string DisplayLink,
    [property: JsonPropertyName("snippet")] string Snippet,
    [property: JsonPropertyName("pagemap")] GoogleWebSearchPageMap Pagemap);

public record GoogleWebSearchPageMap(
    [property: JsonPropertyName("cse_image")] IReadOnlyList<GoogleWebSearchImage>? MainImages,
    [property: JsonPropertyName("cse_thumbnail")] IReadOnlyList<GoogleWebSearchImage>? Thumbnails);

public record GoogleWebSearchImage(
    [property: JsonPropertyName("src")] string Src,
    [property: JsonPropertyName("width")] string Width,
    [property: JsonPropertyName("height")] string Height)
{
    public WebSearchImage ToWebSearchImage() => new(new Uri(Src), Width, Height);
}
