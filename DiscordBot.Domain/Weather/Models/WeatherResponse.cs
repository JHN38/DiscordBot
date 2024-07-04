namespace DiscordBot.Domain.Weather.Models;

public class WeatherResponse
{
    public Location Location { get; set; } = new();

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? IconUrl { get; set; }

    public Temperature Temperature { get; set; } = new();

    public int Pressure { get; set; }

    public int Humidity { get; set; }

    public int Visibility { get; set; }

    public int Clouds { get; set; }

    public DateTime DateTime { get; set; }

    public long Sunrise { get; set; }

    public long Sunset { get; set; }

    public Wind Wind { get; set; } = new();
}

public class Location
{
    public int Id { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public int Timezone { get; set; }
}

public class Temperature
{
    public double Temp { get; set; }

    public double FeelsLike { get; set; }

    public double TempMin { get; set; }

    public double TempMax { get; set; }
}

public class Wind
{
    public double Speed { get; set; }

    public int Deg { get; set; }
}