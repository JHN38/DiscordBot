using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using DiscordBot.Tests.Integration.Fixtures;
using DiscordBot.Tests.Unit.Builders;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DiscordBot.Tests.Integration.RecordKeeping;

/// <summary>
/// Integration tests for DiscordEntityManager using real SQL Server via Testcontainers.
/// These tests validate database behavior, FK constraints, and entity lifecycle.
/// </summary>
[Collection("SqlServer")]
[Trait("Category", "Integration")]
public sealed class DiscordEntityManagerTests : IntegrationTestBase
{
    public DiscordEntityManagerTests(SqlServerFixture fixture) : base(fixture) { }

    #region Happy Path Tests

    [Fact]
    public async Task Message_with_dependencies_persists_entire_graph()
    {
        // Arrange
        var guildDto = new DiscordGuildDto(100, "Test Guild");
        var userDto = new DiscordUserDto(200, "TestUser", "0");
        var channelDto = new DiscordChannelDto(300, "general", guildDto);
        var messageDto = new DiscordMessageDto(
            DiscordId: 400,
            Content: "Hello World",
            Timestamp: DateTimeOffset.UtcNow,
            Author: userDto,
            Channel: channelDto);

        // Act
        var result = await EntityManager.GetOrAddMessageAsync(messageDto);

        // Assert
        await using var db = await GetQueryContextAsync();

        var savedMessage = await db.Messages
            .Include(m => m.Author)
            .Include(m => m.Channel)
            .Include(m => m.Guild)
            .FirstOrDefaultAsync(m => m.Id == result.Id);

        savedMessage.Should().NotBeNull();
        savedMessage!.DiscordId.Should().Be(400UL);
        savedMessage.Content.Should().Be("Hello World");
        savedMessage.Author.Should().NotBeNull();
        savedMessage.Author!.DiscordId.Should().Be(200UL);
        savedMessage.Channel.Should().NotBeNull();
        savedMessage.Channel!.DiscordId.Should().Be(300UL);
        savedMessage.Guild.Should().NotBeNull();
        savedMessage.Guild!.DiscordId.Should().Be(100UL);
    }

    [Fact]
    public async Task Existing_dependencies_are_reused()
    {
        // Arrange - Seed existing entities
        await using (var seedDb = await GetQueryContextAsync())
        {
            seedDb.Guilds.Add(new DiscordGuild { DiscordId = 100, Name = "Existing Guild" });
            seedDb.Users.Add(new DiscordUser { DiscordId = 200, Username = "ExistingUser", Discriminator = "0" });
            seedDb.Channels.Add(new DiscordChannel { DiscordId = 300, Name = "existing-channel" });
            await seedDb.SaveChangesAsync();
        }

        var guildDto = new DiscordGuildDto(100, "Renamed Guild");
        var userDto = new DiscordUserDto(200, "RenamedUser", "9999");
        var channelDto = new DiscordChannelDto(300, "renamed-channel", guildDto);
        var messageDto = new DiscordMessageDto(
            DiscordId: 500,
            Content: "Message with existing deps",
            Timestamp: DateTimeOffset.UtcNow,
            Author: userDto,
            Channel: channelDto);

        // Act
        await EntityManager.GetOrAddMessageAsync(messageDto);

        // Assert - No duplicates created
        await using var db = await GetQueryContextAsync();
        (await db.Guilds.CountAsync()).Should().Be(1);
        (await db.Users.CountAsync()).Should().Be(1);
        (await db.Channels.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task DM_message_has_no_guild()
    {
        // Arrange
        var userDto = new DiscordUserDto(200, "TestUser", "0");
        var channelDto = new DiscordChannelDto(300, "DM Channel", null); // No guild
        var messageDto = new DiscordMessageDto(
            DiscordId: 400,
            Content: "Hello via DM",
            Timestamp: DateTimeOffset.UtcNow,
            Author: userDto,
            Channel: channelDto);

        // Act
        var result = await EntityManager.GetOrAddMessageAsync(messageDto);

        // Assert
        await using var db = await GetQueryContextAsync();
        var savedMessage = await db.Messages
            .Include(m => m.Guild)
            .FirstOrDefaultAsync(m => m.Id == result.Id);

        savedMessage.Should().NotBeNull();
        savedMessage!.Guild.Should().BeNull();
        (await db.Guilds.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Bot_user_flag_is_persisted()
    {
        // Arrange
        var guildDto = new DiscordGuildDto(100, "Test Guild");
        var botUserDto = new DiscordUserDto(200, "BotUser", "0", IsBot: true);
        var channelDto = new DiscordChannelDto(300, "general", guildDto);
        var messageDto = new DiscordMessageDto(
            DiscordId: 400,
            Content: "Bot message",
            Timestamp: DateTimeOffset.UtcNow,
            Author: botUserDto,
            Channel: channelDto);

        // Act
        await EntityManager.GetOrAddMessageAsync(messageDto);

        // Assert
        await using var db = await GetQueryContextAsync();
        var user = await db.Users.FirstOrDefaultAsync(u => u.DiscordId == 200);
        
        user.Should().NotBeNull();
        user!.IsBot.Should().BeTrue();
    }

    [Fact]
    public async Task Same_message_twice_returns_existing()
    {
        // Arrange
        var guildDto = new DiscordGuildDto(100, "Test Guild");
        var userDto = new DiscordUserDto(200, "TestUser", "0");
        var channelDto = new DiscordChannelDto(300, "general", guildDto);
        var messageDto = new DiscordMessageDto(
            DiscordId: 400,
            Content: "Duplicate test",
            Timestamp: DateTimeOffset.UtcNow,
            Author: userDto,
            Channel: channelDto);

        // Act
        var first = await EntityManager.GetOrAddMessageAsync(messageDto);
        var second = await EntityManager.GetOrAddMessageAsync(messageDto);

        // Assert
        first.Id.Should().Be(second.Id);
        first.DiscordId.Should().Be(second.DiscordId);

        await using var db = await GetQueryContextAsync();
        (await db.Messages.CountAsync()).Should().Be(1);
    }

    #endregion

    #region Negative Path Tests

    [Fact]
    public async Task Role_without_guild_throws_InvalidOperationException()
    {
        // Arrange - No guild with ID 999 exists
        var roleDto = new DiscordRoleDto(
            DiscordId: 500,
            Name: "Orphan Role",
            Color: 0xFF0000,
            IsHoisted: false,
            Position: 1,
            Permissions: 0,
            IsManaged: false,
            IsMentionable: false);

        // Act & Assert
        var act = async () => await EntityManager.GetOrAddRoleAsync(roleDto, guildDiscordId: 999);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Guild*999*");
    }

    [Fact]
    public async Task Attachment_without_message_throws_InvalidOperationException()
    {
        // Arrange - No message with ID 999 exists
        var attachmentDto = new DiscordAttachmentDto(
            DiscordId: 500,
            MessageDiscordId: 999, // Non-existent
            Filename: "test.png",
            Url: "https://example.com/test.png",
            Size: 1024);

        // Act & Assert
        var act = async () => await EntityManager.GetOrAddAttachmentAsync(attachmentDto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Message*999*");
    }

    [Fact]
    public async Task Reaction_on_missing_message_throws()
    {
        // Arrange - Create user but no message
        await using (var seedDb = await GetQueryContextAsync())
        {
            seedDb.Users.Add(new DiscordUser { DiscordId = 200, Username = "User", Discriminator = "0" });
            await seedDb.SaveChangesAsync();
        }

        var reactionDto = new DiscordReactionDto(
            EmojiName: "👍",
            MessageDiscordId: 999, // Non-existent
            UserDiscordId: 200);

        // Act & Assert
        var act = async () => await EntityManager.GetOrAddReactionAsync(reactionDto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Message*999*");
    }

    #endregion

    #region Guild Tests

    [Fact]
    public async Task Guild_creates_new_entity()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();

        // Act
        var result = await EntityManager.GetOrAddGuildAsync(guildDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.DiscordId.Should().Be(guildDto.DiscordId);

        await using var db = await GetQueryContextAsync();
        var saved = await db.Guilds.FirstOrDefaultAsync(g => g.Id == result.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be(guildDto.Name);
    }

    [Fact]
    public async Task Guild_returns_existing_when_already_exists()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();

        // Act
        var first = await EntityManager.GetOrAddGuildAsync(guildDto);
        var second = await EntityManager.GetOrAddGuildAsync(guildDto);

        // Assert
        first.Id.Should().Be(second.Id);
        (await CountAsync<DiscordGuild>()).Should().Be(1);
    }

    #endregion

    #region User Tests

    [Fact]
    public async Task User_creates_new_entity()
    {
        // Arrange
        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();

        // Act
        var result = await EntityManager.GetOrAddUserAsync(userDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.DiscordId.Should().Be(userDto.DiscordId);

        await using var db = await GetQueryContextAsync();
        var saved = await db.Users.FirstOrDefaultAsync(u => u.Id == result.Id);
        saved.Should().NotBeNull();
        saved!.Username.Should().Be(userDto.Username);
    }

    [Fact]
    public async Task User_returns_existing_when_already_exists()
    {
        // Arrange
        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();

        // Act
        var first = await EntityManager.GetOrAddUserAsync(userDto);
        var second = await EntityManager.GetOrAddUserAsync(userDto);

        // Assert
        first.Id.Should().Be(second.Id);
        (await CountAsync<DiscordUser>()).Should().Be(1);
    }

    #endregion

    #region Channel Tests

    [Fact]
    public async Task Channel_creates_with_guild()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        var channelDto = DiscordChannelDtoBuilder.CreateUnique()
            .InGuild(guildDto)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddChannelAsync(channelDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);

        await using var db = await GetQueryContextAsync();
        var saved = await db.Channels.Include(c => c.Guild).FirstOrDefaultAsync(c => c.Id == result.Id);
        saved.Should().NotBeNull();
        saved!.Guild.Should().NotBeNull();
        saved.Guild!.DiscordId.Should().Be(guildDto.DiscordId);
    }

    [Fact]
    public async Task Channel_creates_without_guild_for_DM()
    {
        // Arrange
        var channelDto = DiscordChannelDtoBuilder.CreateUnique().AsDM().Build();

        // Act
        var result = await EntityManager.GetOrAddChannelAsync(channelDto);

        // Assert
        await using var db = await GetQueryContextAsync();
        var saved = await db.Channels.Include(c => c.Guild).FirstOrDefaultAsync(c => c.Id == result.Id);
        saved.Should().NotBeNull();
        saved!.Guild.Should().BeNull();
    }

    [Fact]
    public async Task Channel_returns_existing_when_already_exists()
    {
        // Arrange
        var channelDto = DiscordChannelDtoBuilder.CreateUnique().AsDM().Build();

        // Act
        var first = await EntityManager.GetOrAddChannelAsync(channelDto);
        var second = await EntityManager.GetOrAddChannelAsync(channelDto);

        // Assert
        first.Id.Should().Be(second.Id);
        (await CountAsync<DiscordChannel>()).Should().Be(1);
    }

    #endregion

    #region Role Tests

    [Fact]
    public async Task Role_creates_when_guild_exists()
    {
        // Arrange - Create guild first
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var roleDto = DiscordRoleDtoBuilder.CreateUnique().Build();

        // Act
        var result = await EntityManager.GetOrAddRoleAsync(roleDto, guildDto.DiscordId);

        // Assert
        result.Id.Should().BeGreaterThan(0);

        await using var db = await GetQueryContextAsync();
        var saved = await db.Roles.Include(r => r.Guild).FirstOrDefaultAsync(r => r.Id == result.Id);
        saved.Should().NotBeNull();
        saved!.Guild.Should().NotBeNull();
        saved.Guild!.DiscordId.Should().Be(guildDto.DiscordId);
    }

    [Fact]
    public async Task Role_returns_existing_when_already_exists()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);
        var roleDto = DiscordRoleDtoBuilder.CreateUnique().Build();

        // Act
        var first = await EntityManager.GetOrAddRoleAsync(roleDto, guildDto.DiscordId);
        var second = await EntityManager.GetOrAddRoleAsync(roleDto, guildDto.DiscordId);

        // Assert
        first.Id.Should().Be(second.Id);
        (await CountAsync<DiscordRole>()).Should().Be(1);
    }

    #endregion

    #region Invite Tests

    [Fact]
    public async Task Invite_creates_when_dependencies_exist()
    {
        // Arrange - Create guild and channel first
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var channelDto = DiscordChannelDtoBuilder.CreateUnique().InGuild(guildDto).Build();
        await EntityManager.GetOrAddChannelAsync(channelDto);

        var inviteDto = DiscordInviteDtoBuilder.CreateUnique()
            .ForGuild(guildDto.DiscordId)
            .ForChannel(channelDto.DiscordId)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddInviteAsync(inviteDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);

        await using var db = await GetQueryContextAsync();
        var saved = await db.Invites
            .Include(i => i.Guild)
            .Include(i => i.Channel)
            .FirstOrDefaultAsync(i => i.Id == result.Id);

        saved.Should().NotBeNull();
        saved!.Guild.Should().NotBeNull();
        saved.Channel.Should().NotBeNull();
    }

    [Fact]
    public async Task Invite_with_inviter_links_user()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var channelDto = DiscordChannelDtoBuilder.CreateUnique().InGuild(guildDto).Build();
        await EntityManager.GetOrAddChannelAsync(channelDto);

        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var inviteDto = DiscordInviteDtoBuilder.CreateUnique()
            .ForGuild(guildDto.DiscordId)
            .ForChannel(channelDto.DiscordId)
            .CreatedBy(userDto.DiscordId)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddInviteAsync(inviteDto);

        // Assert
        await using var db = await GetQueryContextAsync();
        var saved = await db.Invites.Include(i => i.Inviter).FirstOrDefaultAsync(i => i.Id == result.Id);
        saved!.Inviter.Should().NotBeNull();
        saved.Inviter!.DiscordId.Should().Be(userDto.DiscordId);
    }

    [Fact]
    public async Task Invite_without_inviter_has_null_inviter()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var channelDto = DiscordChannelDtoBuilder.CreateUnique().InGuild(guildDto).Build();
        await EntityManager.GetOrAddChannelAsync(channelDto);

        var inviteDto = DiscordInviteDtoBuilder.CreateUnique()
            .ForGuild(guildDto.DiscordId)
            .ForChannel(channelDto.DiscordId)
            .CreatedBy(null)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddInviteAsync(inviteDto);

        // Assert
        await using var db = await GetQueryContextAsync();
        var saved = await db.Invites.Include(i => i.Inviter).FirstOrDefaultAsync(i => i.Id == result.Id);
        saved!.Inviter.Should().BeNull();
    }

    [Fact]
    public async Task Invite_throws_when_guild_missing()
    {
        // Arrange - No guild exists
        var inviteDto = DiscordInviteDtoBuilder.CreateUnique().Build();

        // Act & Assert
        var act = async () => await EntityManager.GetOrAddInviteAsync(inviteDto);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Guild*");
    }

    #endregion

    #region Reaction Tests

    [Fact]
    public async Task Reaction_creates_when_message_and_user_exist()
    {
        // Arrange - Create message (which creates user)
        var messageDto = DiscordMessageDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddMessageAsync(messageDto);

        var reactionDto = DiscordReactionDtoBuilder.CreateUnique()
            .ForMessage(messageDto.DiscordId)
            .ByUser(messageDto.Author.DiscordId)
            .WithEmoji("thumbsup")
            .Build();

        // Act
        var result = await EntityManager.GetOrAddReactionAsync(reactionDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordReaction>()).Should().Be(1);
    }

    [Fact]
    public async Task Reaction_same_emoji_twice_is_idempotent()
    {
        // Arrange
        var messageDto = DiscordMessageDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddMessageAsync(messageDto);

        var reactionDto = DiscordReactionDtoBuilder.CreateUnique()
            .ForMessage(messageDto.DiscordId)
            .ByUser(messageDto.Author.DiscordId)
            .WithEmoji("heart")
            .Build();

        // Act
        var first = await EntityManager.GetOrAddReactionAsync(reactionDto);
        var second = await EntityManager.GetOrAddReactionAsync(reactionDto);

        // Assert
        first.Id.Should().Be(second.Id);
        (await CountAsync<DiscordReaction>()).Should().Be(1);
    }

    [Fact]
    public async Task Reaction_different_emoji_creates_separate_entries()
    {
        // Arrange
        var messageDto = DiscordMessageDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddMessageAsync(messageDto);

        var reaction1 = DiscordReactionDtoBuilder.CreateUnique()
            .ForMessage(messageDto.DiscordId)
            .ByUser(messageDto.Author.DiscordId)
            .WithEmoji("heart")
            .Build();

        var reaction2 = DiscordReactionDtoBuilder.CreateUnique()
            .ForMessage(messageDto.DiscordId)
            .ByUser(messageDto.Author.DiscordId)
            .WithEmoji("thumbsup")
            .Build();

        // Act
        var first = await EntityManager.GetOrAddReactionAsync(reaction1);
        var second = await EntityManager.GetOrAddReactionAsync(reaction2);

        // Assert
        first.Id.Should().NotBe(second.Id);
        (await CountAsync<DiscordReaction>()).Should().Be(2);
    }

    #endregion

    #region Voice Session Tests

    [Fact]
    public async Task VoiceSession_creates_when_dependencies_exist()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var channelDto = DiscordChannelDtoBuilder.CreateUnique().InGuild(guildDto).Build();
        await EntityManager.GetOrAddChannelAsync(channelDto);

        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var sessionDto = DiscordVoiceSessionDtoBuilder.CreateUnique()
            .ForUser(userDto.DiscordId)
            .InChannel(channelDto.DiscordId)
            .InGuild(guildDto.DiscordId)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddVoiceSessionAsync(sessionDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordVoiceSession>()).Should().Be(1);
    }

    [Fact]
    public async Task VoiceSession_same_sessionId_is_idempotent()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var channelDto = DiscordChannelDtoBuilder.CreateUnique().InGuild(guildDto).Build();
        await EntityManager.GetOrAddChannelAsync(channelDto);

        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var sessionDto = DiscordVoiceSessionDtoBuilder.CreateUnique()
            .WithSessionId("fixed-session-id")
            .ForUser(userDto.DiscordId)
            .InChannel(channelDto.DiscordId)
            .InGuild(guildDto.DiscordId)
            .Build();

        // Act
        var first = await EntityManager.GetOrAddVoiceSessionAsync(sessionDto);
        var second = await EntityManager.GetOrAddVoiceSessionAsync(sessionDto);

        // Assert
        first.Id.Should().Be(second.Id);
        (await CountAsync<DiscordVoiceSession>()).Should().Be(1);
    }

    [Fact]
    public async Task VoiceSession_different_sessionId_creates_new()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var channelDto = DiscordChannelDtoBuilder.CreateUnique().InGuild(guildDto).Build();
        await EntityManager.GetOrAddChannelAsync(channelDto);

        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var session1 = DiscordVoiceSessionDtoBuilder.CreateUnique()
            .WithSessionId("session-1")
            .ForUser(userDto.DiscordId)
            .InChannel(channelDto.DiscordId)
            .InGuild(guildDto.DiscordId)
            .Build();

        var session2 = DiscordVoiceSessionDtoBuilder.CreateUnique()
            .WithSessionId("session-2")
            .ForUser(userDto.DiscordId)
            .InChannel(channelDto.DiscordId)
            .InGuild(guildDto.DiscordId)
            .Build();

        // Act
        var first = await EntityManager.GetOrAddVoiceSessionAsync(session1);
        var second = await EntityManager.GetOrAddVoiceSessionAsync(session2);

        // Assert
        first.Id.Should().NotBe(second.Id);
        (await CountAsync<DiscordVoiceSession>()).Should().Be(2);
    }

    [Fact]
    public async Task VoiceSession_throws_when_user_missing()
    {
        // Arrange - Only create guild and channel, not user
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var channelDto = DiscordChannelDtoBuilder.CreateUnique().InGuild(guildDto).Build();
        await EntityManager.GetOrAddChannelAsync(channelDto);

        var sessionDto = DiscordVoiceSessionDtoBuilder.CreateUnique()
            .ForUser(99999) // Non-existent
            .InChannel(channelDto.DiscordId)
            .InGuild(guildDto.DiscordId)
            .Build();

        // Act & Assert
        var act = async () => await EntityManager.GetOrAddVoiceSessionAsync(sessionDto);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*User*");
    }

    #endregion

    #region User Ban Tests

    [Fact]
    public async Task UserBan_creates_when_dependencies_exist()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var banDto = DiscordUserBanDtoBuilder.CreateUnique()
            .ForUser(userDto.DiscordId)
            .InGuild(guildDto.DiscordId)
            .WithReason("Test ban")
            .Build();

        // Act
        var result = await EntityManager.GetOrAddUserBanAsync(banDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordUserBan>()).Should().Be(1);
    }

    [Fact]
    public async Task UserBan_active_ban_is_idempotent()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var banDto = DiscordUserBanDtoBuilder.CreateUnique()
            .ForUser(userDto.DiscordId)
            .InGuild(guildDto.DiscordId)
            .Active(true)
            .Build();

        // Act
        var first = await EntityManager.GetOrAddUserBanAsync(banDto);
        var second = await EntityManager.GetOrAddUserBanAsync(banDto);

        // Assert
        first.Id.Should().Be(second.Id);
        (await CountAsync<DiscordUserBan>()).Should().Be(1);
    }

    [Fact]
    public async Task UserBan_inactive_ban_allows_new_active_ban()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        // Create inactive ban directly
        await using (var db = await GetQueryContextAsync())
        {
            var guild = await db.Guilds.FirstAsync(g => g.DiscordId == guildDto.DiscordId);
            var user = await db.Users.FirstAsync(u => u.DiscordId == userDto.DiscordId);
            db.UserBans.Add(new DiscordUserBan
            {
                User = user,
                Guild = guild,
                BannedAt = DateTimeOffset.UtcNow.AddDays(-7),
                IsActive = false
            });
            await db.SaveChangesAsync();
        }

        var newBanDto = DiscordUserBanDtoBuilder.CreateUnique()
            .ForUser(userDto.DiscordId)
            .InGuild(guildDto.DiscordId)
            .Active(true)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddUserBanAsync(newBanDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordUserBan>()).Should().Be(2);
    }

    #endregion

    #region Presence Log Tests

    [Fact]
    public async Task PresenceLog_creates_when_user_exists()
    {
        // Arrange
        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var presenceDto = DiscordPresenceLogDtoBuilder.CreateUnique()
            .ForUser(userDto.DiscordId)
            .WithStatus(1)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddPresenceLogAsync(presenceDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordPresenceLog>()).Should().Be(1);
    }

    [Fact]
    public async Task PresenceLog_always_creates_new_entry()
    {
        // Arrange
        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var presenceDto = DiscordPresenceLogDtoBuilder.CreateUnique()
            .ForUser(userDto.DiscordId)
            .Build();

        // Act
        var first = await EntityManager.GetOrAddPresenceLogAsync(presenceDto);
        var second = await EntityManager.GetOrAddPresenceLogAsync(presenceDto);

        // Assert - Always creates new entries for history tracking
        first.Id.Should().NotBe(second.Id);
        (await CountAsync<DiscordPresenceLog>()).Should().Be(2);
    }

    #endregion

    #region Scheduled Event Tests

    [Fact]
    public async Task ScheduledEvent_creates_when_guild_exists()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var eventDto = DiscordScheduledEventDtoBuilder.CreateUnique()
            .InGuild(guildDto.DiscordId)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddScheduledEventAsync(eventDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordScheduledEvent>()).Should().Be(1);
    }

    [Fact]
    public async Task ScheduledEvent_with_channel_links_channel()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var channelDto = DiscordChannelDtoBuilder.CreateUnique().InGuild(guildDto).Build();
        await EntityManager.GetOrAddChannelAsync(channelDto);

        var eventDto = DiscordScheduledEventDtoBuilder.CreateUnique()
            .InGuild(guildDto.DiscordId)
            .InChannel(channelDto.DiscordId)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddScheduledEventAsync(eventDto);

        // Assert
        await using var db = await GetQueryContextAsync();
        var saved = await db.ScheduledEvents.Include(e => e.Channel).FirstAsync(e => e.Id == result.Id);
        saved.Channel.Should().NotBeNull();
    }

    [Fact]
    public async Task ScheduledEvent_is_idempotent()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var eventDto = DiscordScheduledEventDtoBuilder.CreateUnique()
            .InGuild(guildDto.DiscordId)
            .Build();

        // Act
        var first = await EntityManager.GetOrAddScheduledEventAsync(eventDto);
        var second = await EntityManager.GetOrAddScheduledEventAsync(eventDto);

        // Assert
        first.Id.Should().Be(second.Id);
        (await CountAsync<DiscordScheduledEvent>()).Should().Be(1);
    }

    #endregion

    #region Attachment Tests

    [Fact]
    public async Task Attachment_creates_when_message_exists()
    {
        // Arrange
        var messageDto = DiscordMessageDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddMessageAsync(messageDto);

        var attachmentDto = DiscordAttachmentDtoBuilder.CreateUnique()
            .ForMessage(messageDto.DiscordId)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddAttachmentAsync(attachmentDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordAttachment>()).Should().Be(1);
    }

    [Fact]
    public async Task Attachment_is_idempotent()
    {
        // Arrange
        var messageDto = DiscordMessageDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddMessageAsync(messageDto);

        var attachmentDto = DiscordAttachmentDtoBuilder.CreateUnique()
            .ForMessage(messageDto.DiscordId)
            .Build();

        // Act
        var first = await EntityManager.GetOrAddAttachmentAsync(attachmentDto);
        var second = await EntityManager.GetOrAddAttachmentAsync(attachmentDto);

        // Assert
        first.Id.Should().Be(second.Id);
        (await CountAsync<DiscordAttachment>()).Should().Be(1);
    }

    #endregion

    #region Embed Tests

    [Fact]
    public async Task Embed_creates_when_message_exists()
    {
        // Arrange
        var messageDto = DiscordMessageDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddMessageAsync(messageDto);

        var embedDto = DiscordEmbedDtoBuilder.CreateUnique()
            .ForMessage(messageDto.DiscordId)
            .WithTitle("Test Embed")
            .Build();

        // Act
        var result = await EntityManager.GetOrAddEmbedAsync(embedDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordEmbed>()).Should().Be(1);
    }

    [Fact]
    public async Task Embed_always_creates_new_entry()
    {
        // Arrange
        var messageDto = DiscordMessageDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddMessageAsync(messageDto);

        var embedDto = DiscordEmbedDtoBuilder.CreateUnique()
            .ForMessage(messageDto.DiscordId)
            .Build();

        // Act
        var first = await EntityManager.GetOrAddEmbedAsync(embedDto);
        var second = await EntityManager.GetOrAddEmbedAsync(embedDto);

        // Assert - Embeds don't have unique constraints, always create new
        first.Id.Should().NotBe(second.Id);
        (await CountAsync<DiscordEmbed>()).Should().Be(2);
    }

    [Fact]
    public async Task Embed_throws_when_message_missing()
    {
        // Arrange
        var embedDto = DiscordEmbedDtoBuilder.CreateUnique()
            .ForMessage(99999) // Non-existent
            .Build();

        // Act & Assert
        var act = async () => await EntityManager.GetOrAddEmbedAsync(embedDto);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Message*");
    }

    #endregion

    #region Sticker Tests

    [Fact]
    public async Task Sticker_creates_without_dependencies()
    {
        // Arrange - All dependencies are optional
        var stickerDto = DiscordStickerDtoBuilder.CreateUnique().Build();

        // Act
        var result = await EntityManager.GetOrAddStickerAsync(stickerDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordSticker>()).Should().Be(1);
    }

    [Fact]
    public async Task Sticker_with_guild_links_guild()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var stickerDto = DiscordStickerDtoBuilder.CreateUnique()
            .InGuild(guildDto.DiscordId)
            .Build();

        // Act
        var result = await EntityManager.GetOrAddStickerAsync(stickerDto);

        // Assert
        await using var db = await GetQueryContextAsync();
        var saved = await db.Stickers.Include(s => s.Guild).FirstAsync(s => s.Id == result.Id);
        saved.Guild.Should().NotBeNull();
        saved.Guild!.DiscordId.Should().Be(guildDto.DiscordId);
    }

    [Fact]
    public async Task Sticker_is_idempotent()
    {
        // Arrange
        var stickerDto = DiscordStickerDtoBuilder.CreateUnique().Build();

        // Act
        var first = await EntityManager.GetOrAddStickerAsync(stickerDto);
        var second = await EntityManager.GetOrAddStickerAsync(stickerDto);

        // Assert
        first.Id.Should().Be(second.Id);
        (await CountAsync<DiscordSticker>()).Should().Be(1);
    }

    #endregion

    #region User Nickname Tests

    [Fact]
    public async Task UserNickname_creates_when_dependencies_exist()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var nicknameDto = DiscordUserNicknameDtoBuilder.CreateUnique()
            .ForUser(userDto.DiscordId)
            .InGuild(guildDto.DiscordId)
            .WithNickname("TestNick")
            .Build();

        // Act
        var result = await EntityManager.GetOrAddUserNicknameAsync(nicknameDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordUserNickname>()).Should().Be(1);
    }

    [Fact]
    public async Task UserNickname_always_creates_new_entry_for_history()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var nicknameDto = DiscordUserNicknameDtoBuilder.CreateUnique()
            .ForUser(userDto.DiscordId)
            .InGuild(guildDto.DiscordId)
            .Build();

        // Act
        var first = await EntityManager.GetOrAddUserNicknameAsync(nicknameDto);
        var second = await EntityManager.GetOrAddUserNicknameAsync(nicknameDto);

        // Assert - Always creates new entries for history tracking
        first.Id.Should().NotBe(second.Id);
        (await CountAsync<DiscordUserNickname>()).Should().Be(2);
    }

    #endregion

    #region Audit Log Tests

    [Fact]
    public async Task AuditLog_creates_with_all_optional_dependencies_null()
    {
        // Arrange - All dependencies are optional
        var auditLogDto = DiscordAuditLogDtoBuilder.CreateUnique()
            .InGuild(null)
            .PerformedBy(null)
            .Build();

        // Act
        var result = await EntityManager.AddAuditLogAsync(auditLogDto);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        (await CountAsync<DiscordAuditLog>()).Should().Be(1);
    }

    [Fact]
    public async Task AuditLog_links_guild_and_performer_when_exist()
    {
        // Arrange
        var guildDto = DiscordGuildDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddGuildAsync(guildDto);

        var userDto = DiscordUserDtoBuilder.CreateUnique().Build();
        await EntityManager.GetOrAddUserAsync(userDto);

        var auditLogDto = DiscordAuditLogDtoBuilder.CreateUnique()
            .InGuild(guildDto.DiscordId)
            .PerformedBy(userDto.DiscordId)
            .Build();

        // Act
        var result = await EntityManager.AddAuditLogAsync(auditLogDto);

        // Assert
        await using var db = await GetQueryContextAsync();
        var saved = await db.AuditLogs
            .Include(a => a.Guild)
            .Include(a => a.PerformedBy)
            .FirstAsync(a => a.Id == result.Id);

        saved.Guild.Should().NotBeNull();
        saved.PerformedBy.Should().NotBeNull();
    }

    #endregion

    #region Referenced Message Tests

    [Fact]
    public async Task Message_with_referenced_message_links_via_shadow_property()
    {
        // Arrange - Create original message first
        var originalMessage = DiscordMessageDtoBuilder.CreateUnique()
            .WithContent("Original message")
            .Build();
        var originalRef = await EntityManager.GetOrAddMessageAsync(originalMessage);

        // Create reply message
        var replyMessage = DiscordMessageDtoBuilder.CreateUnique()
            .WithContent("This is a reply")
            .InChannel(originalMessage.Channel.DiscordId, originalMessage.Channel.Name, originalMessage.Channel.Guild)
            .WithAuthor(originalMessage.Author)
            .ReplyingTo(originalMessage.DiscordId)
            .Build();

        // Act
        var replyRef = await EntityManager.GetOrAddMessageAsync(replyMessage);

        // Assert
        await using var db = await GetQueryContextAsync();
        var savedReply = await db.Messages.FirstAsync(m => m.Id == replyRef.Id);

        // Check shadow property via raw SQL or entry
        var referencedMessageId = db.Entry(savedReply).Property<int?>("ReferencedMessageId").CurrentValue;
        referencedMessageId.Should().Be(originalRef.Id);
    }

    [Fact]
    public async Task Message_with_nonexistent_referenced_message_creates_without_link()
    {
        // Arrange - Reply to non-existent message
        var replyMessage = DiscordMessageDtoBuilder.CreateUnique()
            .WithContent("Reply to nowhere")
            .ReplyingTo(99999999) // Non-existent
            .Build();

        // Act
        var result = await EntityManager.GetOrAddMessageAsync(replyMessage);

        // Assert - Message should still be created
        result.Id.Should().BeGreaterThan(0);

        await using var db = await GetQueryContextAsync();
        var savedReply = await db.Messages.FirstAsync(m => m.Id == result.Id);

        // Shadow property should be null
        var referencedMessageId = db.Entry(savedReply).Property<int?>("ReferencedMessageId").CurrentValue;
        referencedMessageId.Should().BeNull();
    }

    #endregion
}
