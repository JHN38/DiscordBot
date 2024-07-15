namespace DiscordBot.Application.Common.Configuration;

public class OpenWeatherMapConfig
{
    public string? BaseUrl { get; init; }
    public string? ApiKey { get; init; }
    public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(15);
}
