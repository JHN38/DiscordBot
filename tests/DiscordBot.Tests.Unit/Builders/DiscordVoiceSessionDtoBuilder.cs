using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordVoiceSessionDto instances in tests.
/// </summary>
public sealed class DiscordVoiceSessionDtoBuilder
{
    private string _sessionId = "session-123";
    private ulong _userDiscordId = 1000;
    private ulong _channelDiscordId = 2000;
    private ulong _guildDiscordId = 3000;
    private DateTimeOffset _joinedAt = DateTimeOffset.UtcNow;
    private DateTimeOffset? _leftAt;
    private bool _isSelfMuted;
    private bool _isSelfDeafened;
    private bool _isServerMuted;
    private bool _isServerDeafened;
    private bool _isStreaming;
    private bool _isVideoEnabled;

    public DiscordVoiceSessionDtoBuilder WithSessionId(string sessionId)
    {
        _sessionId = sessionId;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder ForUser(ulong userDiscordId)
    {
        _userDiscordId = userDiscordId;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder InChannel(ulong channelDiscordId)
    {
        _channelDiscordId = channelDiscordId;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder InGuild(ulong guildDiscordId)
    {
        _guildDiscordId = guildDiscordId;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder JoinedAt(DateTimeOffset joinedAt)
    {
        _joinedAt = joinedAt;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder LeftAt(DateTimeOffset? leftAt)
    {
        _leftAt = leftAt;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder SelfMuted(bool muted = true)
    {
        _isSelfMuted = muted;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder SelfDeafened(bool deafened = true)
    {
        _isSelfDeafened = deafened;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder Streaming(bool streaming = true)
    {
        _isStreaming = streaming;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder VideoEnabled(bool enabled = true)
    {
        _isVideoEnabled = enabled;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder ServerMuted(bool muted = true)
    {
        _isServerMuted = muted;
        return this;
    }

    public DiscordVoiceSessionDtoBuilder ServerDeafened(bool deafened = true)
    {
        _isServerDeafened = deafened;
        return this;
    }

    public DiscordVoiceSessionDto Build() => new(
        _sessionId, _userDiscordId, _channelDiscordId, _guildDiscordId, _joinedAt, _leftAt,
        _isSelfMuted, _isSelfDeafened, _isServerMuted, _isServerDeafened,
        _isStreaming, _isVideoEnabled);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordVoiceSessionDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordVoiceSessionDtoBuilder()
            .WithSessionId($"session-{id}")
            .ForUser(id)
            .InChannel(id + 1)
            .InGuild(id + 2);
    }
}
