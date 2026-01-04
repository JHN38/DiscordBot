using System.Collections.Specialized;
using System.Text.Json;
using System.Web;
using DiscordBot.Service.WebSearch.Core.Domain;
using DiscordBot.Service.WebSearch.Core.Interfaces;
using DiscordBot.Service.WebSearch.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordBot.Service.WebSearch.Infrastructure.Services.CustomSearchEngine;

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

        var results = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrEmpty(results))
            return null;

        try
        {
            var searchResult = JsonSerializer.Deserialize<GoogleWebSearchResponse>(results);
            return searchResult?.ToWebSearchResult();
        }
        catch (JsonException ex)
        {
            throw new WebSearchJsonParseException("Failed to parse Google search response.", results, ex);
        }
    }

    public async Task<WebSearchResponse?> SearchAsync(string jsonQuery, CancellationToken cancellationToken = default)
        => await SearchAsync(ConvertJsonToQuery(jsonQuery), cancellationToken);

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

    public static NameValueCollection ConvertJsonToQuery(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
        {
            throw new ArgumentException("JSON string cannot be null or empty.", nameof(jsonString));
        }

        var nameValueCollection = HttpUtility.ParseQueryString(string.Empty);

        try
        {
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString)
                ?? throw new WebSearchJsonParseException("Failed to deserialize JSON query string to dictionary.", jsonString);

            foreach (var (key, value) in dictionary)
            {
                nameValueCollection.Add(key, value);
            }
        }
        catch (JsonException ex)
        {
            throw new WebSearchJsonParseException("Error parsing JSON query string.", jsonString, ex);
        }

        return nameValueCollection;
    }

    public Task<string> SearchImageAsync(IDictionary<string, string> queryParams)
    {
        throw new NotImplementedException();
    }
}
