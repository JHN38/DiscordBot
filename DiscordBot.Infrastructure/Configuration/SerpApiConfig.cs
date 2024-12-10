namespace DiscordBot.Infrastructure.Configuration;
public record SerpApiConfig(string? BaseUrl, string? ApiKey, int MaxResultCount = 10, int DefaultResultCount = 5);
