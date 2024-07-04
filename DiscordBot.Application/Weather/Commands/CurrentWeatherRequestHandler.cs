using MediatR;
using Microsoft.Extensions.Logging;
using CountryData.Standard;
using Discord;
using DiscordBot.Application.Common.Helpers;
using System.Globalization;
using DiscordBot.Domain.Weather.Models;
using DiscordBot.Domain.Weather.Commands;
using DiscordBot.Domain.Weather.Enums;
using DiscordBot.Domain.Weather.Apis;

namespace DiscordBot.Application.WebSearch.Commands;

public class CurrentWeatherRequestHandler(ILogger<CurrentWeatherRequestHandler> logger,
                                          IMediator mediator,
                                          CountryHelper countryHelper) : IRequestHandler<CurrentWeatherRequest, Embed?>
{
    public async Task<Embed?> Handle(CurrentWeatherRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Location))
            return null;

        logger.LogDebug("Fetching current weather for location {Location}", request.Location);

        if (await mediator.Send(new WeatherRequest(WeatherRequestType.Weather, request.Location), cancellationToken) is not List<WeatherResponse> responses || responses.Count == 0)
            return null;

        var weather = responses[0];
        var country = countryHelper.GetCountryByCode(weather.Location.Country);
        var countryFlagUrl = $"https://flagcdn.com/w320/{weather.Location.Country?.ToLower() ?? "01d"}.png";
        var sunrise = TimeHelper.ConvertUnixTimeToLocalTime(weather.Sunrise, country.CountryShortCode);
        var sunset = TimeHelper.ConvertUnixTimeToLocalTime(weather.Sunset, country.CountryShortCode);

        return new EmbedBuilder()
            .WithTitle($"{weather.Title} ({weather.Description})")
            .WithThumbnailUrl(weather.IconUrl)
            .WithAuthor($"{weather.Location.City}, {country.CountryName}", countryFlagUrl)
            .WithDescription("Here's the current weather data:")
            .AddField("🌡️ Temperature", $"{weather.Temperature.Temp} °C", true)
            .AddField("🌡️ Min. Temperature", $"{weather.Temperature.TempMin} °C", true)
            .AddField("🌡️ Max. Temperature", $"{weather.Temperature.TempMax} °C", true)
            .AddField("🌡️ Feels Like", $"{weather.Temperature.FeelsLike} °C", true)
            .AddField("💧 Humidity", $"{weather.Humidity}%", true)
            .AddField("🔼 Pressure", $"{weather.Pressure} hPa", true)
            .AddField("☁️ Cloudiness", $"{weather.Clouds}%", true)
            .AddField("🌬️ Wind Speed", $"{weather.Wind.Speed} m/s", true)
            .AddField("🧭 Wind Direction", $"{weather.Wind.Deg}°", true)
            .AddField("🌅 Sunrise", sunrise.ToString("hh:mm tt", CultureInfo.InvariantCulture), true)
            .AddField("🌇 Sunset", sunset.ToString("hh:mm tt", CultureInfo.InvariantCulture), true)
            .AddField("🕒 Timezone", $"UTC{(weather.Location.Timezone >= 0 ? "+" : "")}{weather.Location.Timezone / 3600}h", true)
            .WithTimestamp(weather.DateTime)
            .Build();
    }
}
