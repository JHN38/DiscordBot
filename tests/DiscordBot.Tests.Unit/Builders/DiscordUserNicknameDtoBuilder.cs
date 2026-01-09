using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordUserNicknameDto instances in tests.
/// </summary>
public sealed class DiscordUserNicknameDtoBuilder
{
    private ulong _userDiscordId = 1000;
    private ulong _guildDiscordId = 2000;
    private string? _nickname = "TestNick";
    private string? _previousNickname;
    private ulong? _changedByDiscordId;

    public DiscordUserNicknameDtoBuilder ForUser(ulong userDiscordId)
    {
        _userDiscordId = userDiscordId;
        return this;
    }

    public DiscordUserNicknameDtoBuilder InGuild(ulong guildDiscordId)
    {
        _guildDiscordId = guildDiscordId;
        return this;
    }

    public DiscordUserNicknameDtoBuilder WithNickname(string? nickname)
    {
        _nickname = nickname;
        return this;
    }

    public DiscordUserNicknameDtoBuilder FromNickname(string? previousNickname)
    {
        _previousNickname = previousNickname;
        return this;
    }

    public DiscordUserNicknameDtoBuilder ChangedBy(ulong? changedByDiscordId)
    {
        _changedByDiscordId = changedByDiscordId;
        return this;
    }

    public DiscordUserNicknameDto Build() => new(
        _userDiscordId, _guildDiscordId, _nickname, _previousNickname, _changedByDiscordId);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordUserNicknameDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordUserNicknameDtoBuilder()
            .ForUser(id)
            .InGuild(id + 1)
            .WithNickname($"Nick-{id}");
    }
}
