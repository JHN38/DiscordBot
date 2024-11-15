using DiscordBot.Domain.Common;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents a user who authored a Discord message.
/// </summary>
public class DiscordUser : DiscordEntity
{
    public required string Username { get; set; }
    public required string Discriminator { get; set; }

    public ICollection<DiscordGuild> Guilds { get; set; } = [];

    /// <summary>
    /// Navigation property for messages authored by this user.
    /// </summary>
    public ICollection<DiscordMessage> Messages { get; set; } = [];
}
