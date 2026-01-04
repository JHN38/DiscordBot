using Microsoft.Extensions.Logging;

namespace DiscordBot.Service.RecordKeeping.Persistence.Services;

/// <summary>
/// Logging definitions for DiscordEntityManager.
/// </summary>
public sealed partial class DiscordEntityManager
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Saving message {DiscordId} from author {AuthorId} in channel {ChannelId}")]
    private partial void LogSavingMessage(ulong discordId, ulong authorId, ulong channelId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Saved message {DiscordId} with database ID {Id}")]
    private partial void LogMessageSaved(ulong discordId, int id);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Linked message to referenced message ID {RefId}")]
    private partial void LogLinkedReferencedMessage(int refId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Referenced message {DiscordId} not found in database")]
    private partial void LogReferencedMessageNotFound(ulong discordId);
}
