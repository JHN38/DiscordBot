namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord message information.
/// Guild is derived from Channel.Guild - dependencies flow: Message → Channel → Guild.
/// </summary>
public sealed record DiscordMessageDto(
    ulong DiscordId,
    string Content,
    DateTimeOffset Timestamp,
    DiscordUserDto Author,
    DiscordChannelDto Channel,
    ulong? ReferencedMessageDiscordId = null,
    bool IsEdited = false,
    DateTimeOffset? EditedTimestamp = null);
