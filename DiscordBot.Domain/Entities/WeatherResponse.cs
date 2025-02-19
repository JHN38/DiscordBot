namespace DiscordBot.Domain.Entities;

public record WeatherResponse(List<WeatherResponseItem> Items);

public record WeatherResponseItem(
    Location Location,
    string? Title,
    string? Description,
    string? IconUrl,
    Temperature Temperature,
    int Pressure,
    int Humidity,
    int Visibility,
    int Clouds,
    DateTime DateTime,
    long Sunrise,
    long Sunset,
    Wind Wind);

public record Location(
    string? City,
    string? Country,
    double Longitude,
    double Latitude,
    int Timezone);

public record Temperature(double Temp, double FeelsLike, double TempMin, double TempMax);

public record Wind(double Speed, int Deg);
