using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Mapping;
using FluentAssertions;
using Xunit;

namespace DiscordBot.Tests.Unit.Mapping;

/// <summary>
/// Edge case tests for CommandMapper.
/// Only tests the MANUAL mapping logic, not auto-generated Mapperly code.
/// </summary>
[Trait("Category", "Unit")]
public sealed class CommandMapperEdgeCaseTests
{
    #region ToMessageDto - Guild Handling

    [Fact]
    public void ToMessageDto_with_null_guild_creates_null_guild_in_channel()
    {
        // Arrange
        var command = new SaveMessageCommand(
            MessageId: 1000,
            ChannelId: 2000,
            ChannelName: "dm-channel",
            GuildId: null,
            GuildName: null,
            AuthorId: 3000,
            AuthorUsername: "TestUser",
            Content: "DM message",
            Timestamp: DateTimeOffset.UtcNow);

        // Act
        var result = command.ToMessageDto();

        // Assert
        result.Channel.Guild.Should().BeNull("DMs have no guild");
    }

    [Fact]
    public void ToMessageDto_with_guildId_but_null_name_creates_null_guild()
    {
        // This is an edge case - GuildId exists but name is null
        var command = new SaveMessageCommand(
            MessageId: 1000,
            ChannelId: 2000,
            ChannelName: "channel",
            GuildId: 5000, // Has ID
            GuildName: null, // But null name
            AuthorId: 3000,
            AuthorUsername: "TestUser",
            Content: "Message",
            Timestamp: DateTimeOffset.UtcNow);

        // Act
        var result = command.ToMessageDto();

        // Assert - Guild should be null because name is required
        result.Channel.Guild.Should().BeNull("guild name is required for guild creation");
    }

    [Fact]
    public void ToMessageDto_with_guild_creates_nested_guild_dto()
    {
        // Arrange
        var command = new SaveMessageCommand(
            MessageId: 1000,
            ChannelId: 2000,
            ChannelName: "general",
            GuildId: 5000,
            GuildName: "Test Guild",
            AuthorId: 3000,
            AuthorUsername: "TestUser",
            Content: "Guild message",
            Timestamp: DateTimeOffset.UtcNow);

        // Act
        var result = command.ToMessageDto();

        // Assert
        result.Channel.Guild.Should().NotBeNull();
        result.Channel.Guild!.DiscordId.Should().Be(5000);
        result.Channel.Guild!.Name.Should().Be("Test Guild");
    }

    #endregion

    #region ToChannelDto - Guild Handling

    [Fact]
    public void ToChannelDto_with_null_guild_creates_null_guild()
    {
        // Arrange
        var command = new SaveChannelCommand(
            ChannelId: 2000,
            Name: "dm-channel",
            GuildId: null,
            GuildName: null);

        // Act
        var result = command.ToChannelDto();

        // Assert
        result.Guild.Should().BeNull();
    }

    [Fact]
    public void ToChannelDto_with_guild_creates_nested_guild()
    {
        // Arrange
        var command = new SaveChannelCommand(
            ChannelId: 2000,
            Name: "general",
            GuildId: 5000,
            GuildName: "Test Guild");

        // Act
        var result = command.ToChannelDto();

        // Assert
        result.Guild.Should().NotBeNull();
        result.Guild!.DiscordId.Should().Be(5000);
    }

    #endregion

    #region ToUserDto - Bot Flag

    [Fact]
    public void ToUserDto_with_bot_flag_sets_isBot()
    {
        // Arrange
        var command = new SaveUserCommand(
            UserId: 3000,
            Username: "BotUser",
            Discriminator: "0",
            IsBot: true);

        // Act
        var result = command.ToUserDto();

        // Assert
        result.IsBot.Should().BeTrue();
    }

    [Fact]
    public void ToUserDto_preserves_optional_properties()
    {
        // Arrange
        var command = new SaveUserCommand(
            UserId: 3000,
            Username: "TestUser",
            Discriminator: "1234",
            IsBot: false,
            GlobalName: "Display Name",
            AvatarUrl: "https://cdn.discord.com/avatar.png");

        // Act
        var result = command.ToUserDto();

        // Assert
        result.GlobalName.Should().Be("Display Name");
        result.AvatarUrl.Should().Be("https://cdn.discord.com/avatar.png");
    }

    #endregion
}
