namespace DiscordBot.Application.Configurations;

public interface IBotConfig
{
    List<ulong>? AdminIds { get; init; }
    bool AlwaysDownloadUsers { get; init; }
    string? TextCommandPrefix { get; init; }
    string? Token { get; init; }
}
