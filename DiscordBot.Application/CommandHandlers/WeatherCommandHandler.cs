using System.Globalization;
using CountryData.Standard;
using Discord;
using DiscordBot.Application.Common.Helpers;
using DiscordBot.Domain.Commands;
using DiscordBot.Domain.Entities;
using DiscordBot.Domain.Enums;
using DiscordBot.Domain.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands;

public class WeatherCommandHandler(ILogger<WeatherCommandHandler> logger,
                                               IMediator mediator,
                                               CountryHelper countryHelper) : IRequestHandler<WeatherCommand, IUserMessage>
{
    public async Task<IUserMessage> Handle(WeatherCommand notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var channel = message.Channel;
        var weatherRequestType = notification.WeatherRequestType;
        var location = notification.Location;
        var units = notification.Units;

        using var typingState = channel.EnterTypingState();

        if (string.IsNullOrWhiteSpace(location))
            return await message.ReplyAsync("No location given.");

        try
        {
            switch (weatherRequestType)
            {
                case "f":
                case "forecast": return await ForecastWeatherEmbed(message, location, units, cancellationToken);
                case "": return await CurrentWeatherEmbed(message, location, units, cancellationToken);
                default: break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving the weather for location {Location}.", location);
            return await message.ReplyAsync("Sorry, I couldn't process your weather request at the moment.");
        }

        logger.LogWarning("Unknown weather request subcommand: {WeatherRequestType}", weatherRequestType);
        return await message.ReplyAsync($"Unknown weather request subcommand: {weatherRequestType}");

    }

    private async Task<IUserMessage> CurrentWeatherEmbed(IUserMessage message, string location, string? units = null, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching weather for location {Location}.", location);

        if (await mediator.Send(new WeatherRequest(WeatherRequestType.Weather, location, units), cancellationToken) is not WeatherResponse response || response.Items.Count == 0)
            return await message.ReplyAsync("Sorry, I couldn't find any weather data for the location you provided.");

        var weather = response.Items[0];
        var country = countryHelper.GetCountryByCode(weather.Location.Country);
        var countryFlagUrl = $"https://flagcdn.com/w320/{weather.Location.Country?.ToLower() ?? "01d"}.png";
        var sunrise = TimeHelper.ConvertUnixTimeToLocalTime(weather.Sunrise, country.CountryShortCode);
        var sunset = TimeHelper.ConvertUnixTimeToLocalTime(weather.Sunset, country.CountryShortCode);

        var embedBuilder = new EmbedBuilder()
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
            .WithTimestamp(weather.DateTime);

        return await message.ReplyAsync(embed: embedBuilder.Build());
    }

    private async Task<IUserMessage> ForecastWeatherEmbed(IUserMessage message, string location, string? units = null, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching forecast for location {Location}.", location);

        if (await mediator.Send(new WeatherRequest(WeatherRequestType.Forecast, location, units), cancellationToken) is not WeatherResponse response || response.Items.Count == 0)
            return await message.ReplyAsync("Sorry, I couldn't find any weather data for the location you provided.");

        var weather = response.Items[0];
        var country = countryHelper.GetCountryByCode(weather.Location.Country);
        var countryFlagUrl = $"https://flagcdn.com/w320/{weather.Location.Country?.ToLower() ?? "US"}.png";

        var embedBuilder = new EmbedBuilder()
            .WithAuthor($"{response.Items[0].Location.City}, {country.CountryName}", countryFlagUrl)
            .WithDescription("Here's the forecast weather data:");

        foreach (var forecasts in response.Items.GroupBy(f => f.DateTime.Date))
        {
            var maxTemperature = forecasts.Max(f => f.Temperature.TempMax);
            var minTemperature = forecasts.Min(f => f.Temperature.TempMin);
            var maxHumidity = forecasts.Max(f => f.Humidity);

            var dayTemperature = $"""
                {Math.Round(maxTemperature)} °C | {Math.Round(minTemperature)} °C                 (H: {maxHumidity}%)
                """;

            if (forecasts.FirstOrDefault() is WeatherResponseItem forecast && forecast.Description is string description)
            {
                var weatherEmoji = WeatherEmojiConverter.ConvertToEmoji(description);
                var dayDescription = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(description);
                dayTemperature = $"{weatherEmoji} {dayDescription}\r\n{dayTemperature}";
            }

            embedBuilder.AddField(forecasts.Max(f => f.DateTime).ToString("dddd"), dayTemperature, false);
        }
        embedBuilder
            .WithTimestamp(response.Items[0].DateTime);

        return await message.ReplyAsync(embed: embedBuilder.Build());
    }
}
