namespace DiscordBot.Application.Common.Configuration;

public class BotOptions
{
    public bool AlwaysDownloadUsers { get; set; }
    public string? Token { get; set; }
    public ulong GuildId { get; set; } = 0x0;
}
