using DiscordBot.Application.Configurations;

namespace DiscordBot.Infrastructure.Configuration;

public record BotConfig : IBotConfig
{
    public List<ulong>? AdminIds { get; init; }
    public string? TextCommandPrefix { get; init; }
    public bool AlwaysDownloadUsers { get; init; }
    public string? Token { get; init; }
}
