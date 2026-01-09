using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordPresenceLogDto instances in tests.
/// </summary>
public sealed class DiscordPresenceLogDtoBuilder
{
    private ulong _userDiscordId = 1000;
    private int _status = 1; // Online
    private DateTimeOffset _timestamp = DateTimeOffset.UtcNow;
    private ulong? _guildDiscordId;
    private int? _previousStatus;
    private string? _clientStatus;
    private int? _activityType;
    private string? _activityName;

    public DiscordPresenceLogDtoBuilder ForUser(ulong userDiscordId)
    {
        _userDiscordId = userDiscordId;
        return this;
    }

    public DiscordPresenceLogDtoBuilder WithStatus(int status)
    {
        _status = status;
        return this;
    }

    public DiscordPresenceLogDtoBuilder At(DateTimeOffset timestamp)
    {
        _timestamp = timestamp;
        return this;
    }

    public DiscordPresenceLogDtoBuilder InGuild(ulong? guildDiscordId)
    {
        _guildDiscordId = guildDiscordId;
        return this;
    }

    public DiscordPresenceLogDtoBuilder FromStatus(int? previousStatus)
    {
        _previousStatus = previousStatus;
        return this;
    }

    public DiscordPresenceLogDtoBuilder WithActivity(int? type, string? name)
    {
        _activityType = type;
        _activityName = name;
        return this;
    }

    public DiscordPresenceLogDtoBuilder WithClientStatus(string? clientStatus)
    {
        _clientStatus = clientStatus;
        return this;
    }

    public DiscordPresenceLogDto Build() => new(
        _userDiscordId, _status, _timestamp, _guildDiscordId,
        _previousStatus, _clientStatus, _activityType, _activityName);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordPresenceLogDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordPresenceLogDtoBuilder()
            .ForUser(id);
    }
}
