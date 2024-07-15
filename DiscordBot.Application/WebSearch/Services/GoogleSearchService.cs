using DiscordBot.Application.Common.Configuration;
using DiscordBot.Application.WebSearch.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Specialized;
using System.Text.Json;
using System.Web;

namespace DiscordBot.Application.WebSearch.Services;

public sealed class GoogleSearchService(HttpClient httpClient, IOptions<GoogleApiConfig> config) : IWebSearchService
{
    private async Task<string> SearchAsync(NameValueCollection queryParameters)
    {
        queryParameters["key"] = config.Value.ApiKey;
        queryParameters["cx"] = config.Value.SearchEngineId;
        queryParameters["fields"] = "items(title,link,displayLink,snippet,pagemap(cse_image,cse_thumbnail))";
        queryParameters["filter"] = "1";

        var uriBuilder = new UriBuilder(config.Value.BaseUrl ?? throw new InvalidOperationException("Base URL is not set."))
        {
            Query = queryParameters.ToString()
        };

        HttpResponseMessage response = await httpClient.GetAsync(uriBuilder.Uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> SearchAsync(string jsonQuery)
        => await SearchAsync(ConvertJsonToQuery(jsonQuery) ?? throw new InvalidOperationException("Failed to convert JSON to query parameters."));

    /// <inheritdoc />
    public async Task<string> SearchAsync(string query, int resultCount, string? location = null, string? language = null, string? hqTerms = null)
    {
        var queryParameters = HttpUtility.ParseQueryString(string.Empty);
        queryParameters["q"] = query;

        // Boost search results whose country of origin matches the parameter value.
        // This will only work in conjunction with the language value setting.
        if (location is string gl)
        {
            queryParameters["gl"] = gl;
        }

        // Restricts the search to documents written in a particular language (e.g., lang_en).
        if (language is string lr)
        {
            queryParameters["lr"] = lr;
        }

        // Appends the specified query terms to the query, as if they were combined with a logical AND operator.
        if (hqTerms is string hq)
        {
            queryParameters["hq"] = hq;
        }

        // Number of search results to return. * Valid values are integers between 1 and 10, inclusive.
        queryParameters["num"] = Math.Clamp(resultCount, 1, config.Value.MaxResultCount).ToString();

        return await SearchAsync(queryParameters);
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
