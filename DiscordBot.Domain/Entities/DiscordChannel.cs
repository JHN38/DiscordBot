using DiscordBot.Domain.Common;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents a channel in which a Discord message was sent.
/// </summary>
public class DiscordChannel : DiscordEntity
{
    public required string Name { get; set; }

    // Foreign Key for Guild
    public ulong GuildId { get; set; }

    /// <summary>
    /// Navigation property for the guild this channel belongs to.
    /// </summary>
    public DiscordGuild Guild { get; set; } = null!;

    /// <summary>
    /// Navigation property for messages sent in this channel.
    /// </summary>
    public ICollection<DiscordMessage> Messages { get; set; } = [];

    /// <summary>
    /// Navigation property for users within this channel.
    /// </summary>
    public ICollection<DiscordUser> Users { get; set; } = [];
}
