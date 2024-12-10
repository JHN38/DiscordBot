namespace DiscordBot.Application.Configurations;

public interface IBotConfig
{
    string Token { get; init; }
    ulong[]? AdminIds { get; init; }
    string? TextCommandPrefix { get; init; }
    bool AlwaysDownloadUsers { get; init; }
}
