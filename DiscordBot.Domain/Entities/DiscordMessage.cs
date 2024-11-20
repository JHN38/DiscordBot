using DiscordBot.Domain.Common;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents a message entity derived from Discord.Net's IMessage, used for storing Discord messages in a database.
/// </summary>
public class DiscordMessage : DiscordEntity
{
    public required string Content { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public bool IsEdited { get; set; }
    public DateTimeOffset? EditedTimestamp { get; set; } = null!;

    // Foreign Key for Author (User)
    public ulong AuthorId { get; set; }

    /// <summary>
    /// Gets or sets the user who authored the message.
    /// </summary>
    public DiscordUser Author { get; set; } = null!;

    // Foreign Key for Channel
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Gets or sets the channel in which the message was sent.
    /// </summary>
    public DiscordChannel Channel { get; set; } = null!;
}
