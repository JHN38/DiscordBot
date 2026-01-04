using System.Globalization;
using CountryData.Standard;
using Discord;
using DiscordBot.Bot.Core.Common.Helpers;
using DiscordBot.Contracts.Weather;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace DiscordBot.Bot.Core.Commands.TextCommands;

public record WeatherCommand(IUserMessage Message, string WeatherRequestType, string Location, string? Units = "metric");

public sealed class WeatherCommandHandler(ILogger<WeatherCommandHandler> logger,
                                            IMessageBus bus,
                                            CountryHelper countryHelper)
{
    public async Task<IUserMessage> Handle(WeatherCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var message = command.Message;
        var channel = message.Channel;
        var weatherRequestType = command.WeatherRequestType;
        var location = command.Location;
        var units = command.Units;

        using var typingState = channel.EnterTypingState();

        if (string.IsNullOrWhiteSpace(location))
            return await message.ReplyAsync("No location given.");

        try
        {
            return weatherRequestType switch
            {
                "f" or "forecast" => await ForecastWeatherEmbed(message, location, units, cancellationToken),
                "" => await CurrentWeatherEmbed(message, location, units, cancellationToken),
                _ => await HandleUnknownWeatherTypeAsync(message, weatherRequestType)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving the weather for location {Location}.", location);
            return await message.ReplyAsync("Sorry, I couldn't process your weather request at the moment.");
        }
    }

    private async Task<IUserMessage> HandleUnknownWeatherTypeAsync(IUserMessage message, string weatherRequestType)
    {
        logger.LogWarning("Unknown weather request subcommand: {WeatherRequestType}", weatherRequestType);
        return await message.ReplyAsync($"Unknown weather request subcommand: {weatherRequestType}");
    }

    private async Task<IUserMessage> CurrentWeatherEmbed(IUserMessage message, string location, string? units = null, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching weather for location {Location}.", location);

        var contractResponse = await bus.InvokeAsync<WeatherResponse>(new GetWeatherQuery(location, "Weather", units), cancellationToken);

        if (contractResponse == null || contractResponse.Items.Count == 0)
            return await message.ReplyAsync("Sorry, I couldn't find any weather data for the location you provided.");

        var weather = contractResponse.Items[0];
        if (weather.Location?.Country is not { } countryCode)
        {
            logger.LogWarning("Weather response missing country code for location {Location}", location);
            return await message.ReplyAsync("Sorry, the weather data is missing location information.");
        }

        var country = countryHelper.GetCountryByCode(countryCode);
#pragma warning disable CA1308 // flagcdn only provides lowercase flag filenames
        var countryFlagUrl = $"https://flagcdn.com/w320/{countryCode.ToLowerInvariant()}.png";
#pragma warning restore CA1308
        var sunrise = TimeHelper.ConvertUnixTimeToLocalTime(weather.Sunrise, country.CountryShortCode);
        var sunset = TimeHelper.ConvertUnixTimeToLocalTime(weather.Sunset, country.CountryShortCode);

        var embedBuilder = new EmbedBuilder()
            .WithTitle($"{weather.Title} ({weather.Description})")
            .WithThumbnailUrl(weather.IconUrl?.ToString())
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

        var contractResponse = await bus.InvokeAsync<WeatherResponse>(new GetWeatherQuery(location, "Forecast", units), cancellationToken);

        if (contractResponse == null || contractResponse.Items.Count == 0)
            return await message.ReplyAsync("Sorry, I couldn't find any weather data for the location you provided.");

        var weather = contractResponse.Items[0];
        if (weather.Location?.Country is not { } countryCode)
        {
            logger.LogWarning("Forecast response missing country code for location {Location}", location);
            return await message.ReplyAsync("Sorry, the weather data is missing location information.");
        }

        var country = countryHelper.GetCountryByCode(countryCode);
#pragma warning disable CA1308 // flagcdn only provides lowercase flag filenames
        var countryFlagUrl = $"https://flagcdn.com/w320/{countryCode.ToLowerInvariant()}.png";
#pragma warning restore CA1308
        var embedBuilder = new EmbedBuilder()
            .WithAuthor($"{contractResponse.Items[0].Location.City}, {country.CountryName}", countryFlagUrl)
            .WithDescription("Here's the forecast weather data:");

        foreach (var forecasts in contractResponse.Items.GroupBy(f => f.DateTime.Date))
        {
            var maxTemperature = forecasts.Max(f => f.Temperature.TempMax);
            var minTemperature = forecasts.Min(f => f.Temperature.TempMin);
            var maxHumidity = forecasts.Max(f => f.Humidity);

            var dayTemperature = $"""
                {Math.Round(maxTemperature)} °C | {Math.Round(minTemperature)} °C                 (H: {maxHumidity}%)
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
            .WithTimestamp(contractResponse.Items[0].DateTime);

        return await message.ReplyAsync(embed: embedBuilder.Build());
    }
}
