using System.Text.Json;
using System.Web;

using DiscordBot.Service.Weather.Core.Domain;
using DiscordBot.Service.Weather.Core.Domain.Enums;
using DiscordBot.Service.Weather.Core.Interfaces;
using DiscordBot.Service.Weather.Infrastructure.Configuration;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot.Service.Weather.Infrastructure.Services;

public sealed class OpenWeatherMapService(ILogger<OpenWeatherMapService> logger,
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
#pragma warning disable CA1308 // OpenWeatherMap expects lowercase in the URL path
        var uriBuilder = new UriBuilder(baseAddress + requestType.ToString().ToLowerInvariant())
        {
            Query = queryParameters.ToString()
        };
#pragma warning restore CA1308

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
        memoryCache.Set(url, response, config.Value.CacheDuration);

        return ProcessResponse(response, requestType);
    }

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
