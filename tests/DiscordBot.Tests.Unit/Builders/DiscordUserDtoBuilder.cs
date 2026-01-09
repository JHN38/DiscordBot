using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordUserDto instances in tests.
/// </summary>
public sealed class DiscordUserDtoBuilder
{
    private ulong _discordId = 2000;
    private string _username = "TestUser";
    private string _discriminator = "0";
    private bool _isBot;
    private string? _globalName;
    private string? _avatarUrl;

    public DiscordUserDtoBuilder WithDiscordId(ulong id)
    {
        _discordId = id;
        return this;
    }

    public DiscordUserDtoBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public DiscordUserDtoBuilder WithDiscriminator(string discriminator)
    {
        _discriminator = discriminator;
        return this;
    }

    public DiscordUserDtoBuilder AsBot(bool isBot = true)
    {
        _isBot = isBot;
        return this;
    }

    public DiscordUserDtoBuilder WithGlobalName(string? globalName)
    {
        _globalName = globalName;
        return this;
    }

    public DiscordUserDtoBuilder WithAvatarUrl(string? avatarUrl)
    {
        _avatarUrl = avatarUrl;
        return this;
    }

    public DiscordUserDto Build() => new(_discordId, _username, _discriminator, _isBot, _globalName, _avatarUrl);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordUserDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordUserDtoBuilder()
            .WithDiscordId(id)
            .WithUsername($"User-{id}");
    }
}
