using System.Collections.Immutable;
using System.Text.Json.Serialization;
using DiscordBot.Domain.Entities;

namespace DiscordBot.Infrastructure.Services.WebSearch.CustomSearchEngine;

public record GoogleWebSearchResponse(
    [property: JsonPropertyName("items")] ImmutableList<GoogleWebSearchResponseItem> Items)
{
    public WebSearchResponse ToWebSearchResult() =>
        new(
            Items.ConvertAll(googleItem => new WebSearchResponseItem(
                googleItem.Title,
                googleItem.Link,
                googleItem.DisplayLink,
                googleItem.Snippet,
                googleItem.Pagemap?.Thumbnails?.ConvertAll(img => img.ToWebSearchImage()) ?? [])));
}

public record GoogleWebSearchResponseItem(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("link")] string Link,
    [property: JsonPropertyName("displayLink")] string DisplayLink,
    [property: JsonPropertyName("snippet")] string Snippet,
    [property: JsonPropertyName("pagemap")] GoogleWebSearchPageMap Pagemap);

public record GoogleWebSearchPageMap(
    [property: JsonPropertyName("cse_image")] List<GoogleWebSearchImage> MainImages,
    [property: JsonPropertyName("cse_thumbnail")] List<GoogleWebSearchImage> Thumbnails);

public record GoogleWebSearchImage(
    [property: JsonPropertyName("src")] string Src,
    [property: JsonPropertyName("width")] string Width,
    [property: JsonPropertyName("height")] string Height)
{
    public WebSearchImage ToWebSearchImage() => new(Src, Width, Height);
}
