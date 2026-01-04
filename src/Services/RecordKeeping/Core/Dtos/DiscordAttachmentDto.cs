namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord attachment information.
/// </summary>
public sealed record DiscordAttachmentDto(
    ulong DiscordId,
    ulong MessageDiscordId,
    string Filename,
    string Url,
    long Size,
    string? ProxyUrl = null,
    int? Width = null,
    int? Height = null,
    string? ContentType = null,
    bool IsSpoiler = false,
    bool IsVoiceMessage = false,
    float? DurationSecs = null,
    string? Waveform = null);
