using System.Text.Json.Serialization;
using DiscordBot.Service.Weather.Core.Domain;

namespace DiscordBot.Service.Weather.Infrastructure.Services;

public sealed record OpenWeatherMapForecastResponseDto
{
    [JsonPropertyName("cod")]
    public string? Cod { get; init; }

    [JsonPropertyName("message")]
    public int Message { get; init; }

    [JsonPropertyName("cnt")]
    public int Cnt { get; init; }

    [JsonPropertyName("list")]
    public IReadOnlyList<Forecast>? List { get; init; }

    [JsonPropertyName("city")]
    public City? City { get; init; }

    public WeatherResponse ToWeatherResponse()
    {
        return new(List?.Select(forecast =>
        {
            var weather = forecast.Weather is { Count: > 0 } ? forecast.Weather[0] : null;
            return new WeatherResponseItem(
                new Location(
                    City?.Name,
                    City?.Country,
                    City?.Coord?.Lon ?? 0,
                    City?.Coord?.Lat ?? 0,
                    City?.Timezone ?? 0
                ),
                weather?.Main,
                weather?.Description,
                weather?.Icon is string icon ? new Uri($"https://openweathermap.org/img/wn/{icon}.png") : null,
            new Temperature(
                forecast.Main?.Temp ?? 0,
                forecast.Main?.FeelsLike ?? 0,
                forecast.Main?.TempMin ?? 0,
                forecast.Main?.TempMax ?? 0
            ),
            forecast.Main?.Pressure ?? 0,
            forecast.Main?.Humidity ?? 0,
            forecast.Visibility,
            forecast.Clouds?.All ?? 0,
            DateTimeOffset.FromUnixTimeSeconds(forecast.Dt).DateTime,
            City?.Sunrise ?? 0,
            City?.Sunset ?? 0,
            new Core.Domain.Wind(
                forecast.Wind?.Speed ?? 0,
                forecast.Wind?.Deg ?? 0
            )
        );
        }).ToList() ?? []);
    }
}

public sealed record Forecast(
    [property: JsonPropertyName("dt")] long Dt = 0,
    [property: JsonPropertyName("main")] Main? Main = null,
    [property: JsonPropertyName("weather")] IReadOnlyList<WeatherDto>? Weather = null,
    [property: JsonPropertyName("clouds")] Clouds? Clouds = null,
    [property: JsonPropertyName("wind")] WindDto? Wind = null,
    [property: JsonPropertyName("visibility")] int Visibility = 0,
    [property: JsonPropertyName("pop")] double Pop = 0,
    [property: JsonPropertyName("sys")] Sys? Sys = null,
    [property: JsonPropertyName("dt_txt")] string? DtTxt = null);

public sealed record City(
    [property: JsonPropertyName("id")] int Id = 0,
    [property: JsonPropertyName("name")] string? Name = null,
    [property: JsonPropertyName("coord")] Coord? Coord = null,
    [property: JsonPropertyName("country")] string? Country = null,
    [property: JsonPropertyName("population")] int Population = 0,
    [property: JsonPropertyName("timezone")] int Timezone = 0,
    [property: JsonPropertyName("sunrise")] long Sunrise = 0,
    [property: JsonPropertyName("sunset")] long Sunset = 0);
