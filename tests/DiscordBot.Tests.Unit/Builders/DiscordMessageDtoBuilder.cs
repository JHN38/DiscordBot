using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordMessageDto instances in tests.
/// </summary>
public sealed class DiscordMessageDtoBuilder
{
    private ulong _discordId = 1000;
    private string _content = "Test message";
    private DateTimeOffset _timestamp = DateTimeOffset.UtcNow;
    private DiscordUserDto _author = new(4000, "TestUser", "0");
    private DiscordChannelDto _channel = new(2000, "general", new DiscordGuildDto(3000, "Test Guild"));
    private ulong? _referencedMessageDiscordId = null;

    public DiscordMessageDtoBuilder WithDiscordId(ulong id)
    {
        _discordId = id;
        return this;
    }

    public DiscordMessageDtoBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    public DiscordMessageDtoBuilder WithTimestamp(DateTimeOffset timestamp)
    {
        _timestamp = timestamp;
        return this;
    }

    public DiscordMessageDtoBuilder WithAuthor(DiscordUserDto author)
    {
        _author = author;
        return this;
    }

    public DiscordMessageDtoBuilder WithAuthor(ulong id, string username, bool isBot = false)
    {
        _author = new DiscordUserDto(id, username, "0", isBot);
        return this;
    }

    public DiscordMessageDtoBuilder InChannel(DiscordChannelDto channel)
    {
        _channel = channel;
        return this;
    }

    public DiscordMessageDtoBuilder InChannel(ulong id, string name, DiscordGuildDto? guild = null)
    {
        _channel = new DiscordChannelDto(id, name, guild);
        return this;
    }

    public DiscordMessageDtoBuilder InGuild(ulong id, string name = "Test Guild")
    {
        _channel = new DiscordChannelDto(_channel.DiscordId, _channel.Name, new DiscordGuildDto(id, name));
        return this;
    }

    public DiscordMessageDtoBuilder AsDM()
    {
        _channel = new DiscordChannelDto(_channel.DiscordId, _channel.Name, null);
        return this;
    }

    public DiscordMessageDtoBuilder ReplyingTo(ulong messageDiscordId)
    {
        _referencedMessageDiscordId = messageDiscordId;
        return this;
    }

    public DiscordMessageDto Build()
    {
        return new DiscordMessageDto(
            DiscordId: _discordId,
            Content: _content,
            Timestamp: _timestamp,
            Author: _author,
            Channel: _channel,
            ReferencedMessageDiscordId: _referencedMessageDiscordId);
    }

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordMessageDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordMessageDtoBuilder()
            .WithDiscordId(id)
            .WithAuthor(id + 1, $"User-{id}")
            .InChannel(id + 2, $"channel-{id}", new DiscordGuildDto(id + 3, $"Guild-{id}"));
    }
}
