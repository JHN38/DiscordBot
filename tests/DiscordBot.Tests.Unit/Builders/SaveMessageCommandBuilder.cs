using DiscordBot.Contracts.RecordKeeping;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating SaveMessageCommand instances in tests.
/// Provides sensible defaults with the ability to customize any property.
/// </summary>
public sealed class SaveMessageCommandBuilder
{
    private ulong _messageId = 1000;
    private ulong _channelId = 2000;
    private string _channelName = "general";
    private ulong? _guildId = 3000;
    private string? _guildName = "Test Guild";
    private ulong _authorId = 4000;
    private string _authorUsername = "TestUser";
    private string _content = "Hello World";
    private DateTimeOffset _timestamp = DateTimeOffset.UtcNow;
    private bool _authorIsBot = false;
    private string? _authorGlobalName = null;
    private string? _authorAvatarUrl = null;

    public SaveMessageCommandBuilder WithMessageId(ulong id)
    {
        _messageId = id;
        return this;
    }

    public SaveMessageCommandBuilder InChannel(ulong id, string name = "test-channel")
    {
        _channelId = id;
        _channelName = name;
        return this;
    }

    public SaveMessageCommandBuilder InGuild(ulong id, string name = "Test Guild")
    {
        _guildId = id;
        _guildName = name;
        return this;
    }

    public SaveMessageCommandBuilder WithAuthor(ulong id, string username = "TestUser")
    {
        _authorId = id;
        _authorUsername = username;
        return this;
    }

    public SaveMessageCommandBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    public SaveMessageCommandBuilder WithTimestamp(DateTimeOffset timestamp)
    {
        _timestamp = timestamp;
        return this;
    }

    public SaveMessageCommandBuilder AsBot(string botName = "TestBot")
    {
        _authorIsBot = true;
        _authorUsername = botName;
        return this;
    }

    public SaveMessageCommandBuilder AsDM()
    {
        _guildId = null;
        _guildName = null;
        return this;
    }

    public SaveMessageCommandBuilder WithGlobalName(string globalName)
    {
        _authorGlobalName = globalName;
        return this;
    }

    public SaveMessageCommandBuilder WithAvatarUrl(string url)
    {
        _authorAvatarUrl = url;
        return this;
    }

    public SaveMessageCommand Build()
    {
        return new SaveMessageCommand(
            MessageId: _messageId,
            ChannelId: _channelId,
            ChannelName: _channelName,
            GuildId: _guildId,
            GuildName: _guildName,
            AuthorId: _authorId,
            AuthorUsername: _authorUsername,
            Content: _content,
            Timestamp: _timestamp,
            AuthorIsBot: _authorIsBot,
            AuthorGlobalName: _authorGlobalName,
            AuthorAvatarUrl: _authorAvatarUrl);
    }

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static SaveMessageCommandBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new SaveMessageCommandBuilder()
            .WithMessageId(id)
            .InChannel(id + 1, $"channel-{id}")
            .InGuild(id + 2, $"Guild-{id}")
            .WithAuthor(id + 3, $"User-{id}");
    }
}
