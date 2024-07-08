using System.Text.Json.Serialization;
using DiscordBot.Domain.Weather.Models;

namespace DiscordBot.Domain.Weather.Apis;

public static partial class OpenWeatherMapExtensions
{
    /// <summary>
    /// Converts an OpenWeatherMapResponse to a WeatherResponse.
    /// </summary>
    /// <param name="openWeatherMapResponse">The OpenWeatherMap response.</param>
    /// <returns>The converted WeatherResponse.</returns>
    public static WeatherResponse ToWeatherResponse(this OpenWeatherMapWeatherResponse openWeatherMapResponse)
    {
        return new WeatherResponse
        {
            Location = new Models.Location
            {
                Id = openWeatherMapResponse.Id,
                City = openWeatherMapResponse.Name,
                Country = openWeatherMapResponse.Sys?.Country,
                Longitude = openWeatherMapResponse.Coord?.Lon ?? 0,
                Latitude = openWeatherMapResponse.Coord?.Lat ?? 0,
                Timezone = openWeatherMapResponse.Timezone
            },
            Title = openWeatherMapResponse.Weather?.FirstOrDefault()?.Main,
            Description = openWeatherMapResponse.Weather?.FirstOrDefault()?.Description,
            IconUrl = $"https://openweathermap.org/img/wn/{openWeatherMapResponse.Weather?.FirstOrDefault()?.Icon ?? "01d"}.png",
            Temperature = new Temperature
            {
                Temp = openWeatherMapResponse.Main?.Temp ?? 0,
                FeelsLike = openWeatherMapResponse.Main?.FeelsLike ?? 0,
                TempMin = openWeatherMapResponse.Main?.TempMin ?? 0,
                TempMax = openWeatherMapResponse.Main?.TempMax ?? 0
            },
            Pressure = openWeatherMapResponse.Main?.Pressure ?? 0,
            Humidity = openWeatherMapResponse.Main?.Humidity ?? 0,
            Visibility = openWeatherMapResponse.Visibility,
            Clouds = openWeatherMapResponse.Clouds?.All ?? 0,
            DateTime = DateTimeOffset.FromUnixTimeSeconds(openWeatherMapResponse.Dt).DateTime,
            Sunrise = openWeatherMapResponse.Sys?.Sunrise ?? 0,
            Sunset = openWeatherMapResponse.Sys?.Sunset ?? 0,
            Wind = new Models.Wind
            {
                Speed = openWeatherMapResponse.Wind?.Speed ?? 0,
                Deg = openWeatherMapResponse.Wind?.Deg ?? 0
            }
        };
    }
}

public class OpenWeatherMapWeatherResponse
{
    [JsonPropertyName("coord")]
    public Coord? Coord { get; set; }

    [JsonPropertyName("weather")]
    public List<Weather>? Weather { get; set; }

    [JsonPropertyName("base")]
    public string? Base { get; set; }

    [JsonPropertyName("main")]
    public Main? Main { get; set; }

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }

    [JsonPropertyName("wind")]
    public Wind? Wind { get; set; }

    [JsonPropertyName("clouds")]
    public Clouds? Clouds { get; set; }

    [JsonPropertyName("dt")]
    public long Dt { get; set; } = 0;

    [JsonPropertyName("sys")]
    public Sys? Sys { get; set; }

    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("cod")]
    public int Cod { get; set; }
}

public class Coord
{
    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("lat")]
    public double Lat { get; set; }
}

public class Weather
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("main")]
    public string? Main { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
}

public class Main
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("temp_min")]
    public double TempMin { get; set; }

    [JsonPropertyName("temp_max")]
    public double TempMax { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("sea_level")]
    public int SeaLevel { get; set; }

    [JsonPropertyName("grnd_level")]
    public int GrndLevel { get; set; }

    [JsonPropertyName("temp_kf")]
    public double TempKf { get; set; }
}

public class Wind
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    [JsonPropertyName("deg")]
    public int Deg { get; set; }

    [JsonPropertyName("gust")]
    public double Gust { get; set; }
}

public class Clouds
{
    [JsonPropertyName("all")]
    public int All { get; set; }
}

public class Sys
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("sunrise")]
    public long Sunrise { get; set; }

    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }

    [JsonPropertyName("pod")]
    public string? Pod { get; set; }
}
