namespace DiscordBot.Service.Weather.Infrastructure.Configuration;

public class OpenWeatherMapConfig
{
    public string? BaseUrl { get; init; }
    public string? ApiKey { get; init; }
    public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(10);
}
