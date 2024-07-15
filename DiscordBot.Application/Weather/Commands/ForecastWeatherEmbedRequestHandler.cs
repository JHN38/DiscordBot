using System.Globalization;
using CountryData.Standard;
using Discord;
using DiscordBot.Application.Weather.Helper;
using DiscordBot.Domain.Weather.Commands;
using DiscordBot.Domain.Weather.Enums;
using DiscordBot.Domain.Weather.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.WebSearch.Commands;

public class ForecastWeatherEmbedRequestHandler(ILogger<ForecastWeatherEmbedRequestHandler> logger,
                                          IMediator mediator,
                                          CountryHelper countryHelper) : IRequestHandler<ForecastWeatherRequest, Embed?>
{
    public async Task<Embed?> Handle(ForecastWeatherRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Location))
            return null;

        logger.LogInformation("Fetching current weather for location {Location}", request.Location);

        if (await mediator.Send(new WeatherRequest(WeatherRequestType.Forecast, request.Location), cancellationToken) is not List<WeatherResponse> responses || responses.Count == 0)
            return null;

        var country = countryHelper.GetCountryByCode(responses[0].Location.Country);
        var countryFlagUrl = $"https://flagcdn.com/w320/{responses[0].Location.Country?.ToLower() ?? "US"}.png";

        var embedBuilder = new EmbedBuilder()
            .WithAuthor($"{responses[0].Location.City}, {country.CountryName}", countryFlagUrl)
            .WithDescription("Here's the forecast weather data:");

        foreach (var forecasts in responses.GroupBy(f => f.DateTime.Date))
        {
            var maxTemperature = forecasts.Max(f => f.Temperature.TempMax);
            var minTemperature = forecasts.Min(f => f.Temperature.TempMin);
            var maxHumidity = forecasts.Max(f => f.Humidity);

            var dayTemperature = $"""
                {Math.Round(maxTemperature)} °C | {Math.Round(minTemperature)} °C                 (H: {maxHumidity}%)
                """;

            if (forecasts.FirstOrDefault() is WeatherResponse forecast && forecast.Description is string description)
            {
                var weatherEmoji = WeatherEmojiConverter.ConvertToEmoji(description);
                var dayDescription = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(description);
                dayTemperature = $"{weatherEmoji} {dayDescription}\r\n{dayTemperature}";
            }

            embedBuilder.AddField(forecasts.Max(f => f.DateTime).ToString("dddd"), dayTemperature, false);
        }
        embedBuilder
            .WithTimestamp(responses[0].DateTime);

        return embedBuilder.Build();
    }
}
