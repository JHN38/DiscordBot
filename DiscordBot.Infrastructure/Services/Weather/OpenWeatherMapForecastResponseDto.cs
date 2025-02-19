using System.Text.Json.Serialization;
using DiscordBot.Domain.Entities;

namespace DiscordBot.Infrastructure.Services;

public class OpenWeatherMapForecastResponseDto
{
    [JsonPropertyName("cod")]
    public string? Cod { get; set; }

    [JsonPropertyName("message")]
    public int Message { get; set; }

    [JsonPropertyName("cnt")]
    public int Cnt { get; set; }

    [JsonPropertyName("list")]
    public List<Forecast>? List { get; set; }

    [JsonPropertyName("city")]
    public City? City { get; set; }

    /// <summary>
    /// Converts an OpenWeatherMapForecastResponse to a list of WeatherResponse.
    /// </summary>
    /// <param name="forecastResponse">The OpenWeatherMap forecast response.</param>
    /// <returns>A list of WeatherResponse.</returns>
    public WeatherResponse ToWeatherResponse()
    {
        return new(List?.Select(forecast => new WeatherResponseItem(
            new Location(
                City?.Name,
                City?.Country,
                City?.Coord?.Lon ?? 0,
                City?.Coord?.Lat ?? 0,
                City?.Timezone ?? 0
            ),
            forecast.Weather?.FirstOrDefault()?.Main,
            forecast.Weather?.FirstOrDefault()?.Description,
            forecast.Weather?.FirstOrDefault()?.Icon,
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
            new Domain.Entities.Wind(
                forecast.Wind?.Speed ?? 0,
                forecast.Wind?.Deg ?? 0
            )
        )).ToList() ?? []);
    }
}

public class Forecast
{
    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("main")]
    public Main? Main { get; set; }

    [JsonPropertyName("weather")]
    public List<Weather>? Weather { get; set; }

    [JsonPropertyName("clouds")]
    public Clouds? Clouds { get; set; }

    [JsonPropertyName("wind")]
    public Wind? Wind { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("pop")]
    public double Pop { get; set; }

    [JsonPropertyName("sys")]
    public Sys? Sys { get; set; }

    [JsonPropertyName("dt_txt")]
    public string? DtTxt { get; set; }
}

public class City
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("coord")]
    public Coord? Coord { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("population")]
    public int Population { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }
}
