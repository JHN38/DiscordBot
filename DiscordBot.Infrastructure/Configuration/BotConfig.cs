namespace DiscordBot.Infrastructure.Configuration;

public record BotConfig
{
    public bool AlwaysDownloadUsers { get; init; }
    public string? Token { get; init; }
}
