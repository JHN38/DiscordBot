namespace DiscordBot.Infrastructure.Configuration;

public record OpenWeatherMapConfig(string? BaseUrl, string? ApiKey, TimeSpan? CacheDuration);
