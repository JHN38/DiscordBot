using System.Text.Json;
using System.Web;
using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Domain.Entities;
using DiscordBot.Domain.Enums;
using DiscordBot.Infrastructure.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot.Infrastructure.Services;

public class OpenWeatherMapService(ILogger<OpenWeatherMapService> logger,
                                   HttpClient httpClient,
                                   IOptions<OpenWeatherMapConfig> config,
                                   IMemoryCache memoryCache) : IWeatherService
{
    public async Task<WeatherResponse?> GetWeatherAsync(WeatherRequestType requestType, string location, string? units = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(location))
            return null;

        logger.LogDebug("Fetching weather for location {Location}", location);

        var queryParameters = HttpUtility.ParseQueryString(string.Empty);
        queryParameters["q"] = location;
        queryParameters["appid"] = config.Value.ApiKey;
        queryParameters["units"] = units ?? WeatherRequestUnits.Metric.ToString();

        var baseAddress = httpClient.BaseAddress ?? throw new InvalidOperationException("BaseUrl was not set.");
        var uriBuilder = new UriBuilder(baseAddress + requestType.ToString().ToLowerInvariant())
        {
            Query = queryParameters.ToString()
        };

        var url = uriBuilder.Uri.ToString();

        // Check if the response is in the cache
        if (memoryCache.TryGetValue(url, out string? cachedResponse))
        {
            logger.LogDebug("Using cached response for location {Location}", location);
            return ProcessResponse(cachedResponse, requestType);
        }

        // Make the HTTP request
        var httpResponse = await httpClient.GetAsync(uriBuilder.Uri, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        var response = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        // Cache the response
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(config.Value.CacheDuration);
        memoryCache.Set(url, response, cacheEntryOptions);

        return ProcessResponse(response, requestType);
    }

    /// <summary>
    /// Processes the JSON response and converts it to a list of WeatherResponse.
    /// </summary>
    /// <param name="response">The JSON response string.</param>
    /// <param name="requestType">The type of the weather request.</param>
    /// <returns>A list of weather responses or null if deserialization fails.</returns>
    private static WeatherResponse? ProcessResponse(string? response, WeatherRequestType requestType)
    {
        if (string.IsNullOrWhiteSpace(response))
            return null;

        return requestType switch
        {
            WeatherRequestType.Weather => JsonSerializer.Deserialize<OpenWeatherMapWeatherResponseDto>(response)?.ToWeatherResponse() ?? null,
            WeatherRequestType.Forecast => JsonSerializer.Deserialize<OpenWeatherMapForecastResponseDto>(response)?.ToWeatherResponse() ?? null,
            _ => null
        };
    }
}
