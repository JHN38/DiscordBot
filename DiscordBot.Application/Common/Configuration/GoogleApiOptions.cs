namespace DiscordBot.Application.Common.Configuration;

public record GoogleApiOptions
{
    public string? ApiKey { get; init; }
    public string? SearchEngineId { get; init; }
    public int MaxResultCount { get; init; } = 5;
}
