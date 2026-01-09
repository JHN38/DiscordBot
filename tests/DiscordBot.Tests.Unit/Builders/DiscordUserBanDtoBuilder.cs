using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordUserBanDto instances in tests.
/// </summary>
public sealed class DiscordUserBanDtoBuilder
{
    private ulong _userDiscordId = 1000;
    private ulong _guildDiscordId = 2000;
    private DateTimeOffset _bannedAt = DateTimeOffset.UtcNow;
    private string? _reason;
    private ulong? _bannedByDiscordId;
    private DateTimeOffset? _unbannedAt;
    private ulong? _unbannedByDiscordId;
    private bool _isActive = true;

    public DiscordUserBanDtoBuilder ForUser(ulong userDiscordId)
    {
        _userDiscordId = userDiscordId;
        return this;
    }

    public DiscordUserBanDtoBuilder InGuild(ulong guildDiscordId)
    {
        _guildDiscordId = guildDiscordId;
        return this;
    }

    public DiscordUserBanDtoBuilder BannedAt(DateTimeOffset bannedAt)
    {
        _bannedAt = bannedAt;
        return this;
    }

    public DiscordUserBanDtoBuilder WithReason(string? reason)
    {
        _reason = reason;
        return this;
    }

    public DiscordUserBanDtoBuilder BannedBy(ulong? bannedByDiscordId)
    {
        _bannedByDiscordId = bannedByDiscordId;
        return this;
    }

    public DiscordUserBanDtoBuilder Unbanned(DateTimeOffset unbannedAt, ulong? unbannedByDiscordId = null)
    {
        _unbannedAt = unbannedAt;
        _unbannedByDiscordId = unbannedByDiscordId;
        _isActive = false;
        return this;
    }

    public DiscordUserBanDtoBuilder Active(bool isActive = true)
    {
        _isActive = isActive;
        return this;
    }

    public DiscordUserBanDto Build() => new(
        _userDiscordId, _guildDiscordId, _bannedAt, _reason,
        _bannedByDiscordId, _unbannedAt, _unbannedByDiscordId, _isActive);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordUserBanDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordUserBanDtoBuilder()
            .ForUser(id)
            .InGuild(id + 1);
    }
}
