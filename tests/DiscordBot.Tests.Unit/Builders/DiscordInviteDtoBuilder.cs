using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordInviteDto instances in tests.
/// </summary>
public sealed class DiscordInviteDtoBuilder
{
    private ulong _discordId = 5000;
    private string _code = "abc123";
    private ulong _guildId = 1000;
    private ulong _channelId = 2000;
    private ulong? _inviterDiscordId;
    private int? _maxAge;
    private int? _maxUses;
    private int _uses;
    private bool _isTemporary;
    private DateTimeOffset? _expiresAt;

    public DiscordInviteDtoBuilder WithDiscordId(ulong id)
    {
        _discordId = id;
        return this;
    }

    public DiscordInviteDtoBuilder WithCode(string code)
    {
        _code = code;
        return this;
    }

    public DiscordInviteDtoBuilder ForGuild(ulong guildId)
    {
        _guildId = guildId;
        return this;
    }

    public DiscordInviteDtoBuilder ForChannel(ulong channelId)
    {
        _channelId = channelId;
        return this;
    }

    public DiscordInviteDtoBuilder CreatedBy(ulong? inviterId)
    {
        _inviterDiscordId = inviterId;
        return this;
    }

    public DiscordInviteDtoBuilder WithMaxAge(int? maxAge)
    {
        _maxAge = maxAge;
        return this;
    }

    public DiscordInviteDtoBuilder WithMaxUses(int? maxUses)
    {
        _maxUses = maxUses;
        return this;
    }

    public DiscordInviteDtoBuilder WithUses(int uses)
    {
        _uses = uses;
        return this;
    }

    public DiscordInviteDtoBuilder Temporary(bool isTemporary = true)
    {
        _isTemporary = isTemporary;
        return this;
    }

    public DiscordInviteDtoBuilder ExpiresAt(DateTimeOffset? expiresAt)
    {
        _expiresAt = expiresAt;
        return this;
    }

    public DiscordInviteDto Build() => new(
        _discordId, _code, _guildId, _channelId, _inviterDiscordId,
        _maxAge, _maxUses, _uses, _isTemporary, _expiresAt);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordInviteDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordInviteDtoBuilder()
            .WithDiscordId(id)
            .WithCode($"invite-{id}")
            .ForGuild(id + 1)
            .ForChannel(id + 2);
    }
}
