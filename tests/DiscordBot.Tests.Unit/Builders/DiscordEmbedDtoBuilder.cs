using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordEmbedDto instances in tests.
/// </summary>
public sealed class DiscordEmbedDtoBuilder
{
    private ulong _messageDiscordId = 1000;
    private string? _type = "rich";
    private string? _title;
    private string? _description;
    private string? _url;
    private DateTimeOffset? _timestamp;
    private int? _color;
    private string? _footerText;
    private string? _imageUrl;
    private string? _authorName;

    public DiscordEmbedDtoBuilder ForMessage(ulong messageDiscordId)
    {
        _messageDiscordId = messageDiscordId;
        return this;
    }

    public DiscordEmbedDtoBuilder WithType(string? type)
    {
        _type = type;
        return this;
    }

    public DiscordEmbedDtoBuilder WithTitle(string? title)
    {
        _title = title;
        return this;
    }

    public DiscordEmbedDtoBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public DiscordEmbedDtoBuilder WithUrl(string? url)
    {
        _url = url;
        return this;
    }

    public DiscordEmbedDtoBuilder WithColor(int? color)
    {
        _color = color;
        return this;
    }

    public DiscordEmbedDtoBuilder WithFooter(string? text)
    {
        _footerText = text;
        return this;
    }

    public DiscordEmbedDtoBuilder WithImage(string? url)
    {
        _imageUrl = url;
        return this;
    }

    public DiscordEmbedDtoBuilder WithAuthor(string? name)
    {
        _authorName = name;
        return this;
    }

    public DiscordEmbedDtoBuilder WithTimestamp(DateTimeOffset? timestamp)
    {
        _timestamp = timestamp;
        return this;
    }

    public DiscordEmbedDto Build() => new(
        _messageDiscordId, _type, _title, _description, _url, _timestamp,
        _color, _footerText, null, _imageUrl, null, null, null, null, _authorName);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordEmbedDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordEmbedDtoBuilder()
            .ForMessage(id)
            .WithTitle($"Embed-{id}");
    }
}
