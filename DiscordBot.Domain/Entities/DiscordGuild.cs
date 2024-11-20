using DiscordBot.Domain.Common;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents a guild (server) where a Discord message was sent.
/// </summary>
public class DiscordGuild : DiscordEntity
{
    public required string Name { get; set; }

    /// <summary>
    /// Navigation property for channels within this guild.
    /// </summary>
    public ICollection<DiscordChannel> Channels { get; set; } = [];

    /// <summary>
    /// Navigation property for users within this guild.
    /// </summary>
    public ICollection<DiscordGuildUser> DiscordGuildUsers { get; set; } = [];
}
