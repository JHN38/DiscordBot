using DiscordBot.Domain.Common;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents a channel in which a Discord message was sent.
/// </summary>
public class DiscordChannel : DiscordEntity
{
    public required string Name { get; set; }

    public ulong GuildId { get; set; }
    public virtual DiscordGuild Guild { get; set; } = null!;

    public virtual ICollection<DiscordMessage> Messages { get; set; } = [];
    public virtual ICollection<DiscordUser> Users { get; set; } = [];
}
