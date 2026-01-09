using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordAuditLogDto instances in tests.
/// </summary>
public sealed class DiscordAuditLogDtoBuilder
{
    private string _entityType = "Message";
    private int _entityId = 1;
    private ulong _entityDiscordId = 1000;
    private string _action = "Created";
    private DateTimeOffset _timestamp = DateTimeOffset.UtcNow;
    private string? _changeType;
    private string? _oldValue;
    private string? _newValue;
    private string? _reason;
    private ulong? _guildDiscordId;
    private ulong? _performedByDiscordId;

    public DiscordAuditLogDtoBuilder WithEntityType(string entityType)
    {
        _entityType = entityType;
        return this;
    }

    public DiscordAuditLogDtoBuilder WithEntityId(int entityId)
    {
        _entityId = entityId;
        return this;
    }

    public DiscordAuditLogDtoBuilder WithEntityDiscordId(ulong entityDiscordId)
    {
        _entityDiscordId = entityDiscordId;
        return this;
    }

    public DiscordAuditLogDtoBuilder WithAction(string action)
    {
        _action = action;
        return this;
    }

    public DiscordAuditLogDtoBuilder At(DateTimeOffset timestamp)
    {
        _timestamp = timestamp;
        return this;
    }

    public DiscordAuditLogDtoBuilder WithChange(string changeType, string? oldValue, string? newValue)
    {
        _changeType = changeType;
        _oldValue = oldValue;
        _newValue = newValue;
        return this;
    }

    public DiscordAuditLogDtoBuilder WithReason(string? reason)
    {
        _reason = reason;
        return this;
    }

    public DiscordAuditLogDtoBuilder InGuild(ulong? guildDiscordId)
    {
        _guildDiscordId = guildDiscordId;
        return this;
    }

    public DiscordAuditLogDtoBuilder PerformedBy(ulong? performedByDiscordId)
    {
        _performedByDiscordId = performedByDiscordId;
        return this;
    }

    public DiscordAuditLogDto Build() => new(
        _entityType, _entityId, _entityDiscordId, _action, _timestamp,
        _changeType, _oldValue, _newValue, _reason, _guildDiscordId, _performedByDiscordId);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordAuditLogDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordAuditLogDtoBuilder()
            .WithEntityDiscordId(id)
            .WithEntityId((int)(id % int.MaxValue));
    }
}
