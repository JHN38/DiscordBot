using Discord;
using DiscordBot.Domain.Entities;

namespace DiscordBot.Domain.Common.Extensions;

/// <summary>
/// Provides extension methods for initializing Discord domain entities from Discord.Net types.
/// </summary>
public static class DiscordEntityExtensions
{
    /// <summary>
    /// Initializes a DiscordUser entity from a Discord.Net IUser instance.
    /// </summary>
    /// <param name="user">The Discord.Net IUser instance.</param>
    /// <returns>A new DiscordUser entity with required properties set.</returns>
    public static DiscordUser ToDiscordUser(this IUser user) =>
        new()
        {
            Id = user.Id,
            Username = user.Username,
            Discriminator = user.Discriminator
        };

    public static DiscordChannel ToDiscordChannel(this IChannel channel) =>
        ToDiscordChannel((IGuildChannel)channel);

    /// <summary>
    /// Initializes a DiscordChannel entity from a Discord.Net IChannel instance.
    /// </summary>
    /// <param name="channel">The Discord.Net IChannel instance.</param>
    /// <param name="guild">The associated DiscordGuild entity.</param>
    /// <returns>A new DiscordChannel entity with required properties set.</returns>
    public static DiscordChannel ToDiscordChannel(this IGuildChannel channel) =>
        new()
        {
            Id = channel.Id,
            Name = channel.Name,
            GuildId = channel.Guild.Id
        };

    /// <summary>
    /// Initializes a DiscordGuild entity from a Discord.Net IGuild instance.
    /// </summary>
    /// <param name="guild">The Discord.Net IGuild instance.</param>
    /// <returns>A new DiscordGuild entity with required properties set.</returns>
    public static DiscordGuild ToDiscordGuild(this IGuild guild) =>
        new()
        {
            Id = guild.Id,
            Name = guild.Name
        };

    /// <summary>
    /// Initializes a DiscordMessage entity from a Discord.Net IMessage instance.
    /// </summary>
    /// <param name="message">The Discord.Net IMessage instance.</param>
    /// <returns>A new DiscordMessage entity with required properties set.</returns>
    public static DiscordMessage ToDiscordMessage(this IMessage message) =>
        new()
        {
            Id = message.Id,
            Content = message.Content,
            Timestamp = message.Timestamp,
            IsEdited = message.EditedTimestamp.HasValue,
            EditedTimestamp = message.EditedTimestamp,
            ChannelId = message.Channel.Id,
            AuthorId = message.Author.Id
        };
}
