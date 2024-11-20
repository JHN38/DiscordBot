using DiscordBot.Domain.Common;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents a user who authored a Discord message.
/// </summary>
public class DiscordUser : DiscordEntity
{
    public required string Username { get; set; }
    public required string Discriminator { get; set; }

    /// <summary>
    /// Navigation property for messages authored by this user.
    /// </summary>
    public ICollection<DiscordMessage> Messages { get; set; } = [];

    public ICollection<DiscordChannelUser> DiscordChannelUsers { get; set; } = [];
    public ICollection<DiscordGuildUser> DiscordGuildUsers { get; set; } = [];
}

public class DiscordChannelUser
{
    public ulong DiscordChannelId { get; set; }
    public DiscordChannel DiscordChannel { get; set; } = null!;

    public ulong UserId { get; set; }
    public DiscordUser User { get; set; } = null!;
}

public class DiscordGuildUser
{
    public ulong DiscordGuildId { get; set; }
    public DiscordGuild DiscordGuild { get; set; } = null!;

    public ulong UserId { get; set; }
    public DiscordUser User { get; set; } = null!;
}
