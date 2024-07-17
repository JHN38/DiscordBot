using System.Text.Json;
using System.Web;
using DiscordBot.Application.Common.Configuration;
using DiscordBot.Domain.Weather.Commands;
using DiscordBot.Domain.Weather.Enums;
using DiscordBot.Domain.Weather.Models;
using DiscordBot.Domain.Weather.Models.OpenWeatherMap;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot.Application.WebSearch.Commands;

/// <summary>
/// Handles requests for fetching weather data from OpenWeatherMap.
/// </summary>
public class OpenWeatherMapRequestHandler(ILogger<OpenWeatherMapRequestHandler> logger,
                                   HttpClient httpClient,
                                   IOptions<OpenWeatherMapConfig> config,
                                   IMemoryCache memoryCache) : IRequestHandler<WeatherRequest, WeatherResponse?>
{
    /// <summary>
    /// Handles the WeatherRequest to fetch weather data.
    /// </summary>
    /// <param name="request">The weather request details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of weather responses or null if the request fails.</returns>
    public async Task<WeatherResponse?> Handle(WeatherRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Location))
            return null;

        logger.LogDebug("Fetching weather for location {Location}", request.Location);

        var requestType = request.RequestType.ToString().ToLowerInvariant();

        var queryParameters = HttpUtility.ParseQueryString(string.Empty);
        queryParameters["q"] = request.Location;
        queryParameters["appid"] = config.Value.ApiKey;
        queryParameters["units"] = request.Units ?? WeatherRequestUnits.Metric.ToString();

        var uriBuilder = new UriBuilder(config.Value.BaseUrl + requestType)
        {
            Query = queryParameters.ToString()
        };

        var url = uriBuilder.Uri.ToString();

        // Check if the response is in the cache
        if (memoryCache.TryGetValue(url, out string? cachedResponse))
        {
            logger.LogDebug("Using cached response for location {Location}", request.Location);
            return ProcessResponse(cachedResponse, request.RequestType);
        }

        // Make the HTTP request
        var httpResponse = await httpClient.GetAsync(uriBuilder.Uri, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();
        var response = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        // Cache the response
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(config.Value.CacheDuration);
        memoryCache.Set(url, response, cacheEntryOptions);

        return ProcessResponse(response, request.RequestType);
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
            WeatherRequestType.Weather => JsonSerializer.Deserialize<OpenWeatherMapWeatherResponse>(response)?.ToWeatherResponse() ?? null,
            WeatherRequestType.Forecast => JsonSerializer.Deserialize<OpenWeatherMapForecastResponse>(response)?.ToWeatherResponse() ?? null,
            _ => null
        };
    }
}
