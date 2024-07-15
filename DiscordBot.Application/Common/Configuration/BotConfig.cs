namespace DiscordBot.Application.Common.Configuration;

public record BotConfig
{
    public bool AlwaysDownloadUsers { get; init; }
    public string? Token { get; init; }
    public ulong GuildId { get; init; } = 0x0;
}
