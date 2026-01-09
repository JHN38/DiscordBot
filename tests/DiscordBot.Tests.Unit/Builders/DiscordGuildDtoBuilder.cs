using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordGuildDto instances in tests.
/// </summary>
public sealed class DiscordGuildDtoBuilder
{
    private ulong _discordId = 1000;
    private string _name = "Test Guild";

    public DiscordGuildDtoBuilder WithDiscordId(ulong id)
    {
        _discordId = id;
        return this;
    }

    public DiscordGuildDtoBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public DiscordGuildDto Build() => new(_discordId, _name);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordGuildDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordGuildDtoBuilder()
            .WithDiscordId(id)
            .WithName($"Guild-{id}");
    }
}
