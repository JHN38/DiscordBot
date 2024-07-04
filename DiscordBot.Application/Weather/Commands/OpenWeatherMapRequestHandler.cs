using MediatR;
using Microsoft.Extensions.Logging;
using DiscordBot.Domain.Weather.Models;
using DiscordBot.Domain.Weather.Commands;
using DiscordBot.Application.Common.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;
using DiscordBot.Domain.Weather.Apis;
using DiscordBot.Domain.Weather.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace DiscordBot.Application.WebSearch.Commands;

/// <summary>
/// Handles requests for fetching weather data from OpenWeatherMap.
/// </summary>
public class OpenWeatherMapRequestHandler(ILogger<OpenWeatherMapRequestHandler> logger,
                                   IHttpClientFactory httpClientFactory,
                                   IOptions<WeatherOptions> weatherOptions,
                                   IMemoryCache memoryCache) : IRequestHandler<WeatherRequest, List<WeatherResponse>?>
{
    /// <summary>
    /// Handles the WeatherRequest to fetch weather data.
    /// </summary>
    /// <param name="request">The weather request details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of weather responses or null if the request fails.</returns>
    public async Task<List<WeatherResponse>?> Handle(WeatherRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Location))
            return null;

        if (httpClientFactory.CreateClient("OpenWeatherMap") is not HttpClient httpClient)
            return null;

        logger.LogDebug("Fetching weather for location {Location}", request.Location);

        var requestType = request.RequestType.ToString().ToLowerInvariant();
        var url = $"data/2.5/{requestType}?q={request.Location}&appid={weatherOptions.Value.ApiKey}&units={request.Units}";

        // Check if the response is in the cache
        if (memoryCache.TryGetValue(url, out string? cachedResponse))
        {
            logger.LogDebug("Using cached response for location {Location}", request.Location);
            return ProcessResponse(cachedResponse, request.RequestType);
        }

        // Make the HTTP request
        if (await httpClient.GetStringAsync(url, cancellationToken) is not string response)
            return null;

        // Cache the response
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(weatherOptions.Value.CacheDuration);
        memoryCache.Set(url, response, cacheEntryOptions);

        return ProcessResponse(response, request.RequestType);
    }

    /// <summary>
    /// Processes the JSON response and converts it to a list of WeatherResponse.
    /// </summary>
    /// <param name="response">The JSON response string.</param>
    /// <param name="requestType">The type of the weather request.</param>
    /// <returns>A list of weather responses or null if deserialization fails.</returns>
    private static List<WeatherResponse>? ProcessResponse(string? response, WeatherRequestType requestType)
    {
        if (string.IsNullOrWhiteSpace(response))
            return null;

        if (requestType == WeatherRequestType.Weather && JsonSerializer.Deserialize<OpenWeatherMapWeatherResponse>(response) is OpenWeatherMapWeatherResponse weatherResponse)
        {
            return [weatherResponse.ToWeatherResponse()];
        }

        if (requestType == WeatherRequestType.Forecast && JsonSerializer.Deserialize<OpenWeatherMapForecastResponse>(response) is OpenWeatherMapForecastResponse forecastResponse)
        {
            return forecastResponse.ToWeatherResponseList();
        }

        return null;
    }
}
