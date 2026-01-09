using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordScheduledEventDto instances in tests.
/// </summary>
public sealed class DiscordScheduledEventDtoBuilder
{
    private ulong _discordId = 1000;
    private string _name = "Test Event";
    private ulong _guildDiscordId = 2000;
    private DateTimeOffset _scheduledStartTime = DateTimeOffset.UtcNow.AddDays(1);
    private int _privacyLevel = 2; // GuildOnly
    private int _status = 1; // Scheduled
    private int _entityType = 3; // External
    private string? _description;
    private DateTimeOffset? _scheduledEndTime;
    private ulong? _channelDiscordId;
    private ulong? _creatorDiscordId;

    public DiscordScheduledEventDtoBuilder WithDiscordId(ulong id)
    {
        _discordId = id;
        return this;
    }

    public DiscordScheduledEventDtoBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public DiscordScheduledEventDtoBuilder InGuild(ulong guildDiscordId)
    {
        _guildDiscordId = guildDiscordId;
        return this;
    }

    public DiscordScheduledEventDtoBuilder StartsAt(DateTimeOffset startTime)
    {
        _scheduledStartTime = startTime;
        return this;
    }

    public DiscordScheduledEventDtoBuilder EndsAt(DateTimeOffset? endTime)
    {
        _scheduledEndTime = endTime;
        return this;
    }

    public DiscordScheduledEventDtoBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public DiscordScheduledEventDtoBuilder InChannel(ulong? channelDiscordId)
    {
        _channelDiscordId = channelDiscordId;
        return this;
    }

    public DiscordScheduledEventDtoBuilder CreatedBy(ulong? creatorDiscordId)
    {
        _creatorDiscordId = creatorDiscordId;
        return this;
    }

    public DiscordScheduledEventDtoBuilder WithPrivacyLevel(int level)
    {
        _privacyLevel = level;
        return this;
    }

    public DiscordScheduledEventDtoBuilder WithStatus(int status)
    {
        _status = status;
        return this;
    }

    public DiscordScheduledEventDtoBuilder WithEntityType(int entityType)
    {
        _entityType = entityType;
        return this;
    }

    public DiscordScheduledEventDto Build() => new(
        _discordId, _name, _guildDiscordId, _scheduledStartTime,
        _privacyLevel, _status, _entityType, _description, _scheduledEndTime,
        null, null, null, _channelDiscordId, _creatorDiscordId);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordScheduledEventDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordScheduledEventDtoBuilder()
            .WithDiscordId(id)
            .WithName($"Event-{id}")
            .InGuild(id + 1);
    }
}
