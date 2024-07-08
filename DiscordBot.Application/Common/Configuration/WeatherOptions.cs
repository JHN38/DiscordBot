namespace DiscordBot.Application.Common.Configuration;

public class WeatherOptions
{
    public string? ApiKey { get; init; }
    public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(15);
}
