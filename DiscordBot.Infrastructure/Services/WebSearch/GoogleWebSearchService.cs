using System.Collections.Specialized;
using System.Text.Json;
using System.Web;
using DiscordBot.Application.Interfaces;
using DiscordBot.Domain.Entities;
using DiscordBot.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordBot.Infrastructure.Services;

public sealed class GoogleWebSearchService(HttpClient httpClient, IOptions<GoogleApiConfig> config) : IWebSearchService
{
    private async Task<WebSearchResponse?> SearchAsync(NameValueCollection queryParameters, CancellationToken cancellationToken = default)
    {
        queryParameters["key"] = config.Value.ApiKey;
        queryParameters["cx"] = config.Value.SearchEngineId;
        queryParameters["fields"] = "items(title,link,displayLink,snippet,pagemap(cse_image,cse_thumbnail))";
        queryParameters["filter"] = "1";

        var baseAddress = httpClient.BaseAddress ?? throw new InvalidOperationException("BaseUrl was not set.");
        var uriBuilder = new UriBuilder(baseAddress)
        {
            Query = queryParameters.ToString()
        };

        var response = await httpClient.GetAsync(uriBuilder.Uri, cancellationToken);
        response.EnsureSuccessStatusCode();

        if (await response.Content.ReadAsStringAsync(cancellationToken) is not string results)
            return null;

        if (JsonSerializer.Deserialize<GoogleWebSearchResponseDto>(results) is not GoogleWebSearchResponseDto searchResult)
            return null;

        return searchResult.ToWebSearchResult();
    }

    public async Task<WebSearchResponse?> SearchAsync(string jsonQuery, CancellationToken cancellationToken = default)
        => await SearchAsync(ConvertJsonToQuery(jsonQuery) ?? throw new InvalidOperationException("Failed to convert JSON to query parameters."), cancellationToken);

    /// <inheritdoc />
    public async Task<WebSearchResponse?> SearchAsync(string query, int? resultCount, string? countryRestriction = null, string? languageRestriction = null, CancellationToken cancellationToken = default)
    {
        var queryParameters = HttpUtility.ParseQueryString(string.Empty);
        queryParameters["q"] = query;

        // Boost search results whose country of origin matches the parameter value.
        // This will only work in conjunction with the language value setting.
        if (countryRestriction is string gl)
        {
            queryParameters["gl"] = gl;
        }

        // Restricts the search to documents written in a particular language (e.g., lang_en).
        if (languageRestriction is string lr)
        {
            queryParameters["lr"] = lr;
        }

        // Number of search results to return. * Valid values are integers between 1 and 10, inclusive.
        queryParameters["num"] = Math.Clamp(resultCount ?? config.Value.DefaultResultCount,
            min: 1, max: config.Value.MaxResultCount).ToString();

        return await SearchAsync(queryParameters, cancellationToken);
    }

    /// <summary>
    /// Converts a JSON string containing a dictionary of key/value pairs to a NameValueCollection.
    /// </summary>
    /// <param name="jsonString">The JSON string representing a dictionary.</param>
    /// <returns>A NameValueCollection containing the key/value pairs from the JSON string.</returns>
    public static NameValueCollection? ConvertJsonToQuery(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
        {
            throw new ArgumentException("JSON string cannot be null or empty.", nameof(jsonString));
        }

        var nameValueCollection = HttpUtility.ParseQueryString(string.Empty);

        try
        {
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString) ??
                             throw new InvalidOperationException("Failed to deserialize JSON string to dictionary.");

            foreach (var (key, value) in dictionary)
            {
                nameValueCollection.Add(key, value);
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Error parsing JSON string.", ex);
        }

        return nameValueCollection;
    }
}
