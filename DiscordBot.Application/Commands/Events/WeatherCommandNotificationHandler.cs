using CountryData.Standard;
using System.Globalization;
using Discord;
using DiscordBot.Application.Common.Helpers;
using DiscordBot.Application.Weather.Helper;
using DiscordBot.Domain.Commands.Events;
using DiscordBot.Domain.Weather.Commands;
using DiscordBot.Domain.Weather.Enums;
using DiscordBot.Domain.Weather.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Discord.WebSocket;

namespace DiscordBot.Application.Commands.Events;

public class WeatherCommandNotificationHandler(ILogger<WeatherCommandNotificationHandler> logger,
                                               IMediator mediator,
                                               CountryHelper countryHelper) : INotificationHandler<WeatherCommandNotification>
{
    public async Task Handle(WeatherCommandNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var location = notification.Location;

        if (string.IsNullOrWhiteSpace(location))
        {
            await message.ReplyAsync("No location given.");
            return;
        }

        if (await mediator.Send(new WeatherRequest(WeatherRequestType.Weather, location), cancellationToken) is not WeatherResponse response || response.Items.Count == 0)
        {
            await message.ReplyAsync("Sorry, I couldn't find any weather data for the location you provided.");
            return;
        }

        logger.LogInformation("Fetching weather for location {Location}, {Country}.", response.Items[0].Location.City, response.Items[0].Location.City);

        using (message.Channel.EnterTypingState())
        {
            try
            {
                switch (notification.WeatherRequestType)
                {
                    case "": await CurrentWeatherEmbed(message, response); break;
                    case "f":
                    case "forecast": await ForecastWeatherEmbed(message, response); break;
                    default:
                        logger.LogWarning("Unknown weather request subcommand: {WeatherRequestType}", notification.WeatherRequestType);
                        await message.ReplyAsync($"Unknown weather request subcommand: {notification.WeatherRequestType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the weather for location  {Location}, {Country}.", response.Items[0].Location.City, response.Items[0].Location.City);
                await message.ReplyAsync("Sorry, I couldn't process your search request at the moment.");
            }
        }
    }

    private Task<IUserMessage> CurrentWeatherEmbed(SocketUserMessage message, WeatherResponse response)
    {
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

        return message.ReplyAsync(embed: embedBuilder.Build());
    }

    private Task<IUserMessage> ForecastWeatherEmbed(SocketUserMessage message, WeatherResponse response)
    {
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

        return message.ReplyAsync(embed: embedBuilder.Build());
    }
}
