namespace DiscordBot.Application.Common.Configuration;

public record GoogleApiConfig
{
    public string? BaseUrl { get; init; }
    public string? ApiKey { get; init; }
    public string? SearchEngineId { get; init; }
    public int MaxResultCount { get; init; } = 5;
}
