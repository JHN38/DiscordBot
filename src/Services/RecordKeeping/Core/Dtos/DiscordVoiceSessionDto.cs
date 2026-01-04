namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord voice session information.
/// </summary>
public sealed record DiscordVoiceSessionDto(
    string SessionId,
    ulong UserDiscordId,
    ulong ChannelDiscordId,
    ulong GuildDiscordId,
    DateTimeOffset JoinedAt,
    DateTimeOffset? LeftAt = null,
    bool IsSelfMuted = false,
    bool IsSelfDeafened = false,
    bool IsServerMuted = false,
    bool IsServerDeafened = false,
    bool IsStreaming = false,
    bool IsVideoEnabled = false,
    bool IsSuppressed = false,
    DateTimeOffset? RequestToSpeakTimestamp = null);
