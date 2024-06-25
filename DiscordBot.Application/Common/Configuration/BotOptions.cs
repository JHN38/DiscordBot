namespace DiscordBot.Application.Common.Configuration;

public record BotOptions
{
    public bool AlwaysDownloadUsers { get; init; }
    public string? Token { get; init; }
    public ulong GuildId { get; init; } = 0x0;
}
