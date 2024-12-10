namespace DiscordBot.Infrastructure.Configuration;

public record GoogleApiConfig(string? BaseUrl, string? ApiKey, string? SearchEngineId, int MaxResultCount = 10, int DefaultResultCount = 5);

