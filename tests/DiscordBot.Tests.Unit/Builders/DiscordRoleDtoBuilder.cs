using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;
using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordRoleDto instances in tests.
/// </summary>
public sealed class DiscordRoleDtoBuilder
{
    private ulong _discordId = 4000;
    private string _name = "Test Role";
    private int _color;
    private bool _isHoisted;
    private int _position = 1;
    private DiscordPermissions _permissions = DiscordPermissions.None;
    private bool _isManaged;
    private bool _isMentionable = true;
    private string? _iconUrl;
    private string? _unicodeEmoji;
    private string? _tags;

    public DiscordRoleDtoBuilder WithDiscordId(ulong id)
    {
        _discordId = id;
        return this;
    }

    public DiscordRoleDtoBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public DiscordRoleDtoBuilder WithColor(int color)
    {
        _color = color;
        return this;
    }

    public DiscordRoleDtoBuilder Hoisted(bool isHoisted = true)
    {
        _isHoisted = isHoisted;
        return this;
    }

    public DiscordRoleDtoBuilder WithPosition(int position)
    {
        _position = position;
        return this;
    }

    public DiscordRoleDtoBuilder WithPermissions(DiscordPermissions permissions)
    {
        _permissions = permissions;
        return this;
    }

    public DiscordRoleDtoBuilder Managed(bool isManaged = true)
    {
        _isManaged = isManaged;
        return this;
    }

    public DiscordRoleDtoBuilder Mentionable(bool isMentionable = true)
    {
        _isMentionable = isMentionable;
        return this;
    }

    public DiscordRoleDtoBuilder WithIconUrl(string? iconUrl)
    {
        _iconUrl = iconUrl;
        return this;
    }

    public DiscordRoleDtoBuilder WithUnicodeEmoji(string? unicodeEmoji)
    {
        _unicodeEmoji = unicodeEmoji;
        return this;
    }

    public DiscordRoleDtoBuilder WithTags(string? tags)
    {
        _tags = tags;
        return this;
    }

    public DiscordRoleDto Build() => new(
        _discordId, _name, _color, _isHoisted, _position,
        _permissions, _isManaged, _isMentionable, _iconUrl, _unicodeEmoji, _tags);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordRoleDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordRoleDtoBuilder()
            .WithDiscordId(id)
            .WithName($"Role-{id}");
    }
}
