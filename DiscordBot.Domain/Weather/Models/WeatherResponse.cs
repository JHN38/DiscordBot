namespace DiscordBot.Domain.Weather.Models;

public record WeatherResponse
{
    public Location Location { get; init; } = new();

    public string? Title { get; init; }

    public string? Description { get; init; }

    public string? IconUrl { get; init; }

    public Temperature Temperature { get; init; } = new();

    public int Pressure { get; init; }

    public int Humidity { get; init; }

    public int Visibility { get; init; }

    public int Clouds { get; init; }

    public DateTime DateTime { get; init; }

    public long Sunrise { get; init; }

    public long Sunset { get; init; }

    public Wind Wind { get; init; } = new();
}

public record Location
{
    public int Id { get; init; }

    public string? City { get; init; }

    public string? Country { get; init; }

    public double Longitude { get; init; }

    public double Latitude { get; init; }

    public int Timezone { get; init; }
}

public record Temperature
{
    public double Temp { get; init; }

    public double FeelsLike { get; init; }

    public double TempMin { get; init; }

    public double TempMax { get; init; }
}

public record Wind
{
    public double Speed { get; init; }

    public int Deg { get; init; }
}