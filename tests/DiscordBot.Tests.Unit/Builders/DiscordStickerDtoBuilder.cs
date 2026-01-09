using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordStickerDto instances in tests.
/// </summary>
public sealed class DiscordStickerDtoBuilder
{
    private ulong _discordId = 1000;
    private string _name = "Test Sticker";
    private int _formatType = 1; // PNG
    private bool _isAvailable = true;
    private string? _description;
    private string? _tags;
    private ulong? _guildDiscordId;
    private ulong? _creatorDiscordId;

    public DiscordStickerDtoBuilder WithDiscordId(ulong id)
    {
        _discordId = id;
        return this;
    }

    public DiscordStickerDtoBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public DiscordStickerDtoBuilder WithFormatType(int formatType)
    {
        _formatType = formatType;
        return this;
    }

    public DiscordStickerDtoBuilder Available(bool isAvailable = true)
    {
        _isAvailable = isAvailable;
        return this;
    }

    public DiscordStickerDtoBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public DiscordStickerDtoBuilder WithTags(string? tags)
    {
        _tags = tags;
        return this;
    }

    public DiscordStickerDtoBuilder InGuild(ulong? guildDiscordId)
    {
        _guildDiscordId = guildDiscordId;
        return this;
    }

    public DiscordStickerDtoBuilder CreatedBy(ulong? creatorDiscordId)
    {
        _creatorDiscordId = creatorDiscordId;
        return this;
    }

    public DiscordStickerDto Build() => new(
        _discordId, _name, _formatType, _isAvailable,
        _description, _tags, null, null, _guildDiscordId, _creatorDiscordId);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordStickerDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordStickerDtoBuilder()
            .WithDiscordId(id)
            .WithName($"Sticker-{id}");
    }
}
