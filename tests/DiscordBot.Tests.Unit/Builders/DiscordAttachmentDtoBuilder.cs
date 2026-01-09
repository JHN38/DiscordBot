using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordAttachmentDto instances in tests.
/// </summary>
public sealed class DiscordAttachmentDtoBuilder
{
    private ulong _discordId = 1000;
    private ulong _messageDiscordId = 2000;
    private string _filename = "test.png";
    private string _url = "https://cdn.discord.com/attachments/test.png";
    private long _size = 1024;
    private string? _proxyUrl;
    private int? _width;
    private int? _height;
    private string? _contentType;
    private bool _isSpoiler;
    private bool _isVoiceMessage;

    public DiscordAttachmentDtoBuilder WithDiscordId(ulong id)
    {
        _discordId = id;
        return this;
    }

    public DiscordAttachmentDtoBuilder ForMessage(ulong messageDiscordId)
    {
        _messageDiscordId = messageDiscordId;
        return this;
    }

    public DiscordAttachmentDtoBuilder WithFilename(string filename)
    {
        _filename = filename;
        return this;
    }

    public DiscordAttachmentDtoBuilder WithUrl(string url)
    {
        _url = url;
        return this;
    }

    public DiscordAttachmentDtoBuilder WithSize(long size)
    {
        _size = size;
        return this;
    }

    public DiscordAttachmentDtoBuilder AsImage(int width, int height, string contentType = "image/png")
    {
        _width = width;
        _height = height;
        _contentType = contentType;
        return this;
    }

    public DiscordAttachmentDtoBuilder AsSpoiler(bool isSpoiler = true)
    {
        _isSpoiler = isSpoiler;
        return this;
    }

    public DiscordAttachmentDtoBuilder AsVoiceMessage(bool isVoice = true)
    {
        _isVoiceMessage = isVoice;
        return this;
    }

    public DiscordAttachmentDtoBuilder WithProxyUrl(string? proxyUrl)
    {
        _proxyUrl = proxyUrl;
        return this;
    }

    public DiscordAttachmentDto Build() => new(
        _discordId, _messageDiscordId, _filename, _url, _size,
        _proxyUrl, _width, _height, _contentType, _isSpoiler, _isVoiceMessage);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordAttachmentDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordAttachmentDtoBuilder()
            .WithDiscordId(id)
            .ForMessage(id + 1)
            .WithFilename($"file-{id}.png")
            .WithUrl($"https://cdn.discord.com/attachments/{id}.png");
    }
}
