using DiscordBot.Application.Configurations;

namespace DiscordBot.Infrastructure.Configuration;

public record BotConfig : IBotConfig
{
    public required string Token { get; init; }
    public ulong[]? AdminIds { get; init; }
    public string? TextCommandPrefix { get; init; }
    public bool AlwaysDownloadUsers { get; init; } = true;
}
