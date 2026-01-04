using System.Text.Json.Serialization;
using DiscordBot.Service.Weather.Core.Domain;

namespace DiscordBot.Service.Weather.Infrastructure.Services;

public sealed record OpenWeatherMapWeatherResponseDto
{
    [JsonPropertyName("coord")]
    public Coord? Coord { get; init; }

    [JsonPropertyName("weather")]
    public IReadOnlyList<WeatherDto>? Weather { get; init; }

    [JsonPropertyName("base")]
    public string? Base { get; init; }

    [JsonPropertyName("main")]
    public Main? Main { get; init; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; init; }

    [JsonPropertyName("wind")]
    public WindDto? Wind { get; init; }

    [JsonPropertyName("clouds")]
    public Clouds? Clouds { get; init; }

    [JsonPropertyName("dt")]
    public long Dt { get; init; }

    [JsonPropertyName("sys")]
    public Sys? Sys { get; init; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; init; }

    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("cod")]
    public int Cod { get; init; }

    public WeatherResponse ToWeatherResponse()
    {
        var weather = Weather is { Count: > 0 } ? Weather[0] : null;
        return new(
            [new(
                new Location(
                    Name,
                    Sys?.Country,
                    Coord?.Lon ?? 0,
                    Coord?.Lat ?? 0,
                    Timezone
                ),
                weather?.Main,
                weather?.Description,
                new Uri($"https://openweathermap.org/img/wn/{weather?.Icon ?? "01d"}.png"),
                new Temperature(
                    Main?.Temp ?? 0,
                    Main?.FeelsLike ?? 0,
                    Main?.TempMin ?? 0,
                    Main?.TempMax ?? 0
                ),
                Main?.Pressure ?? 0,
                Main?.Humidity ?? 0,
                Visibility,
                Clouds?.All ?? 0,
                DateTimeOffset.FromUnixTimeSeconds(Dt).DateTime,
                Sys?.Sunrise ?? 0,
                Sys?.Sunset ?? 0,
                new Core.Domain.Wind(
                    Wind?.Speed ?? 0,
                    Wind?.Deg ?? 0
                )
            )]
        );
    }
}

public sealed record Coord(
    [property: JsonPropertyName("lon")] double Lon = 0,
    [property: JsonPropertyName("lat")] double Lat = 0);

public sealed record WeatherDto(
    [property: JsonPropertyName("id")] int Id = 0,
    [property: JsonPropertyName("main")] string? Main = null,
    [property: JsonPropertyName("description")] string? Description = null,
    [property: JsonPropertyName("icon")] string? Icon = null);

public sealed record Main(
    [property: JsonPropertyName("temp")] double Temp = 0,
    [property: JsonPropertyName("feels_like")] double FeelsLike = 0,
    [property: JsonPropertyName("temp_min")] double TempMin = 0,
    [property: JsonPropertyName("temp_max")] double TempMax = 0,
    [property: JsonPropertyName("pressure")] int Pressure = 0,
    [property: JsonPropertyName("humidity")] int Humidity = 0,
    [property: JsonPropertyName("sea_level")] int SeaLevel = 0,
    [property: JsonPropertyName("grnd_level")] int GrndLevel = 0,
    [property: JsonPropertyName("temp_kf")] double TempKf = 0);

public sealed record WindDto(
    [property: JsonPropertyName("speed")] double Speed = 0,
    [property: JsonPropertyName("deg")] int Deg = 0,
    [property: JsonPropertyName("gust")] double Gust = 0);

public sealed record Clouds(
    [property: JsonPropertyName("all")] int All = 0);

public sealed record Sys(
    [property: JsonPropertyName("type")] int Type = 0,
    [property: JsonPropertyName("id")] int Id = 0,
    [property: JsonPropertyName("country")] string? Country = null,
    [property: JsonPropertyName("sunrise")] long Sunrise = 0,
    [property: JsonPropertyName("sunset")] long Sunset = 0,
    [property: JsonPropertyName("pod")] string? Pod = null);
