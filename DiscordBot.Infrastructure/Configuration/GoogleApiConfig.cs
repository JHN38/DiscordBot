namespace DiscordBot.Infrastructure.Configuration;

public record GoogleApiConfig
{
    public string? BaseUrl { get; init; }
    public string? ApiKey { get; init; }
    public string? SearchEngineId { get; init; }
    public int MaxResultCount { get; init; } = 10;
    public int DefaultResultCount { get; init; } = 5;
}
