﻿using System.Text.Json.Serialization;
using DiscordBot.Domain.Weather.Interfaces;

namespace DiscordBot.Domain.Weather.Models.OpenWeatherMap;

public class OpenWeatherMapWeatherResponse : IApiWeatherResponse
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

    /// <summary>
    /// Converts an OpenWeatherMapResponse to a WeatherResponse.
    /// </summary>
    /// <param name="openWeatherMapResponse">The OpenWeatherMap response.</param>
    /// <returns>The converted WeatherResponse.</returns>
    public List<WeatherResponse> ToWeatherResponseList()
    {
        return [new WeatherResponse
        {
            Location = new Models.Location
            {
                Id = Id,
                City = Name,
                Country = Sys?.Country,
                Longitude = Coord?.Lon ?? 0,
                Latitude = Coord?.Lat ?? 0,
                Timezone = Timezone
            },
            Title = Weather?.FirstOrDefault()?.Main,
            Description = Weather?.FirstOrDefault()?.Description,
            IconUrl = $"https://openweathermap.org/img/wn/{Weather?.FirstOrDefault()?.Icon ?? "01d"}.png",
            Temperature = new Temperature
            {
                Temp = Main?.Temp ?? 0,
                FeelsLike = Main?.FeelsLike ?? 0,
                TempMin = Main?.TempMin ?? 0,
                TempMax = Main?.TempMax ?? 0
            },
            Pressure = Main?.Pressure ?? 0,
            Humidity = Main?.Humidity ?? 0,
            Visibility = Visibility,
            Clouds = Clouds?.All ?? 0,
            DateTime = DateTimeOffset.FromUnixTimeSeconds(Dt).DateTime,
            Sunrise = Sys?.Sunrise ?? 0,
            Sunset = Sys?.Sunset ?? 0,
            Wind = new Models.Wind
            {
                Speed = Wind?.Speed ?? 0,
                Deg = Wind?.Deg ?? 0
            }
        }];
    }
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