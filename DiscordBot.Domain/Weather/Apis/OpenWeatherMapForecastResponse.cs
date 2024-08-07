﻿using System.Text.Json.Serialization;
using DiscordBot.Domain.Weather.Models;

namespace DiscordBot.Domain.Weather.Apis;

public static partial class OpenWeatherMapExtensions
{
    /// <summary>
    /// Converts an OpenWeatherMapForecastResponse to a list of WeatherResponse.
    /// </summary>
    /// <param name="forecastResponse">The OpenWeatherMap forecast response.</param>
    /// <returns>A list of WeatherResponse.</returns>
    public static List<WeatherResponse> ToWeatherResponseList(this OpenWeatherMapForecastResponse forecastResponse)
    {
        return forecastResponse.List?.Select(forecast => new WeatherResponse
        {
            Location = new Location
            {
                Id = forecastResponse.City?.Id ?? 0,
                City = forecastResponse.City?.Name,
                Country = forecastResponse.City?.Country,
                Longitude = forecastResponse.City?.Coord?.Lon ?? 0,
                Latitude = forecastResponse.City?.Coord?.Lat ?? 0,
                Timezone = forecastResponse.City?.Timezone ?? 0
            },
            Title = forecast.Weather?.FirstOrDefault()?.Main,
            Description = forecast.Weather?.FirstOrDefault()?.Description,
            IconUrl = forecast.Weather?.FirstOrDefault()?.Icon,
            Temperature = new Temperature
            {
                Temp = forecast.Main?.Temp ?? 0,
                FeelsLike = forecast.Main?.FeelsLike ?? 0,
                TempMin = forecast.Main?.TempMin ?? 0,
                TempMax = forecast.Main?.TempMax ?? 0
            },
            Pressure = forecast.Main?.Pressure ?? 0,
            Humidity = forecast.Main?.Humidity ?? 0,
            Visibility = forecast.Visibility,
            Clouds = forecast.Clouds?.All ?? 0,
            DateTime = DateTimeOffset.FromUnixTimeSeconds(forecast.Dt).DateTime,
            Sunrise = forecastResponse.City?.Sunrise ?? 0,
            Sunset = forecastResponse.City?.Sunset ?? 0,
            Wind = new Models.Wind
            {
                Speed = forecast.Wind?.Speed ?? 0,
                Deg = forecast.Wind?.Deg ?? 0
            }
        }).ToList() ?? [];
    }
}

public class OpenWeatherMapForecastResponse
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
