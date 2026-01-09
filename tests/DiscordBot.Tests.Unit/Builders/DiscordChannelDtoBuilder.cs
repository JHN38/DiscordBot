using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordChannelDto instances in tests.
/// </summary>
public sealed class DiscordChannelDtoBuilder
{
    private ulong _discordId = 3000;
    private string _name = "general";
    private DiscordGuildDto? _guild;

    public DiscordChannelDtoBuilder WithDiscordId(ulong id)
    {
        _discordId = id;
        return this;
    }

    public DiscordChannelDtoBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public DiscordChannelDtoBuilder InGuild(DiscordGuildDto? guild)
    {
        _guild = guild;
        return this;
    }

    public DiscordChannelDtoBuilder InGuild(ulong guildId, string guildName = "Test Guild")
    {
        _guild = new DiscordGuildDto(guildId, guildName);
        return this;
    }

    public DiscordChannelDtoBuilder AsDM()
    {
        _guild = null;
        return this;
    }

    public DiscordChannelDto Build() => new(_discordId, _name, _guild);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordChannelDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordChannelDtoBuilder()
            .WithDiscordId(id)
            .WithName($"channel-{id}")
            .InGuild(id + 1, $"Guild-{id + 1}");
    }
}
