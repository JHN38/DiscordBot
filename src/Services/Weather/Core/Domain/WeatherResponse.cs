namespace DiscordBot.Service.Weather.Core.Domain;

public record WeatherResponse(IReadOnlyList<WeatherResponseItem> Items);

public record WeatherResponseItem(
    Location Location,
    string? Title,
    string? Description,
    Uri? IconUrl,
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
