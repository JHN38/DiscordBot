using DiscordBot.Domain.Common;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents a user who authored a Discord message.
/// </summary>
public class DiscordUser : DiscordEntity
{
    public required string Username { get; set; }
    public required string Discriminator { get; set; }

    public virtual ICollection<DiscordMessage> Messages { get; set; } = [];
    public virtual ICollection<DiscordChannel> Channels { get; set; } = [];
    public virtual ICollection<DiscordGuild> Guilds { get; set; } = [];
}

public readonly record struct DiscordGuildUserPair(ulong GuildId, ulong UserId);
