using DiscordBot.Service.RecordKeeping.Core.Domain.Common;
using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;

namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

public class DiscordMessage : DiscordEntity
{
    public required string Content { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public bool IsEdited { get; set; }
    public DateTimeOffset? EditedTimestamp { get; set; }

    // New properties from schema expansion
    public MessageType MessageType { get; set; }
    public bool IsPinned { get; set; }
    public bool IsTTS { get; set; }
    public bool MentionedEveryone { get; set; }
    public string? MentionedUserIds { get; set; } // JSON array of mentioned user IDs
    public string? MentionedRoleIds { get; set; } // JSON array of mentioned role IDs
    public string? MentionedChannelIds { get; set; } // JSON array of mentioned channel IDs
    public int Flags { get; set; } // Message flags bitfield
    public ulong? ApplicationId { get; set; } // Application ID if from bot/app
    public ulong? WebhookId { get; set; } // Webhook ID if from webhook
    public ulong? InteractionId { get; set; } // Interaction ID if response
    public ulong? ThreadId { get; set; } // Thread ID if started thread
    public bool IsDeleted { get; set; } // Soft delete flag
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// The guild this message was sent in. Null for DM messages.
    /// </summary>
    public virtual DiscordGuild? Guild { get; set; }

    /// <summary>
    /// The author of this message. Set via entity manager, not Mapperly.
    /// </summary>
    public virtual DiscordUser Author { get; set; } = default!;

    /// <summary>
    /// The channel this message was sent in. Set via entity manager, not Mapperly.
    /// </summary>
    public virtual DiscordChannel Channel { get; set; } = default!;

    public virtual DiscordMessage? ReferencedMessage { get; set; }

    public virtual ICollection<DiscordMessage> ReferencedByMessages { get; } = [];

    // New collections from schema expansion
    public virtual ICollection<DiscordReaction> Reactions { get; } = [];
    public virtual ICollection<DiscordAttachment> Attachments { get; } = [];
    public virtual ICollection<DiscordEmbed> Embeds { get; } = [];
    public virtual ICollection<DiscordSticker> Stickers { get; } = []; // Via MessageStickers
}
