using DiscordBot.Service.RecordKeeping.Core.Dtos;

namespace DiscordBot.Tests.Unit.Builders;

/// <summary>
/// Fluent builder for creating DiscordReactionDto instances in tests.
/// </summary>
public sealed class DiscordReactionDtoBuilder
{
    private ulong _messageDiscordId = 1000;
    private ulong _userDiscordId = 2000;
    private string _emojiName = "thumbsup";
    private ulong? _emojiId;
    private bool _isCustomEmoji;
    private bool _isAnimated;
    private bool _isBurst;

    public DiscordReactionDtoBuilder ForMessage(ulong messageDiscordId)
    {
        _messageDiscordId = messageDiscordId;
        return this;
    }

    public DiscordReactionDtoBuilder ByUser(ulong userDiscordId)
    {
        _userDiscordId = userDiscordId;
        return this;
    }

    public DiscordReactionDtoBuilder WithEmoji(string name, ulong? id = null)
    {
        _emojiName = name;
        _emojiId = id;
        _isCustomEmoji = id.HasValue;
        return this;
    }

    public DiscordReactionDtoBuilder CustomEmoji(ulong emojiId, string name, bool animated = false)
    {
        _emojiName = name;
        _emojiId = emojiId;
        _isCustomEmoji = true;
        _isAnimated = animated;
        return this;
    }

    public DiscordReactionDtoBuilder Burst(bool isBurst = true)
    {
        _isBurst = isBurst;
        return this;
    }

    public DiscordReactionDto Build() => new(
        _messageDiscordId, _userDiscordId, _emojiName,
        _emojiId, _isCustomEmoji, _isAnimated, _isBurst);

    /// <summary>
    /// Creates a builder with unique IDs to avoid conflicts in parallel tests.
    /// </summary>
    public static DiscordReactionDtoBuilder CreateUnique()
    {
        var id = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        return new DiscordReactionDtoBuilder()
            .ForMessage(id)
            .ByUser(id + 1)
            .WithEmoji($"emoji-{id}");
    }
}
