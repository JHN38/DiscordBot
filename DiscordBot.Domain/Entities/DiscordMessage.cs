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

    public ulong GuildId { get; set; }
    public virtual DiscordGuild Guild { get; set; } = null!;

    public ulong AuthorId { get; set; }
    public virtual DiscordUser Author { get; set; } = null!;

    public ulong ChannelId { get; set; }
    public virtual DiscordChannel Channel { get; set; } = null!;

    public ulong? ReferencedMessageId { get; set; }
    public virtual DiscordMessage? ReferencedMessage { get; set; } = null!;

    public virtual ICollection<DiscordMessage> ReferencedByMessages { get; set; } = [];
}
