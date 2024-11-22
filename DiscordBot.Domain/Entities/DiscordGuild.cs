using DiscordBot.Domain.Common;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents a guild (server) where a Discord message was sent.
/// </summary>
public class DiscordGuild : DiscordEntity
{
    public required string Name { get; set; }

    public virtual ICollection<DiscordChannel> Channels { get; set; } = [];
    public virtual ICollection<DiscordUser> Users { get; set; } = [];
    public virtual ICollection<DiscordMessage> Messages { get; set; } = [];
}
