using DiscordBot.Service.RecordKeeping.Core.Dtos;
using DiscordBot.Tests.Integration.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DiscordBot.Tests.Integration.RecordKeeping;

/// <summary>
/// Tests for concurrent operations on DiscordEntityManager.
/// Discord sends events in parallel - these tests verify the manager handles race conditions.
/// 
/// NOTE: These tests INTENTIONALLY FAIL to expose real concurrency bugs in DiscordEntityManager.
/// The failures are legitimate - duplicate key violations occur when parallel requests
/// try to create the same entity. This requires implementing retry/upsert logic in EntityManager.
/// </summary>
[Collection("SqlServer")]
[Trait("Category", "Integration")]
public sealed class ConcurrencyTests : IntegrationTestBase
{
    public ConcurrencyTests(SqlServerFixture fixture) : base(fixture) { }

    [Fact]
    public async Task Concurrent_messages_from_same_user_create_single_user()
    {
        // Arrange
        const int parallelCount = 10;
        var userDiscordId = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        
        var tasks = Enumerable.Range(0, parallelCount).Select(i =>
        {
            var guildDto = new DiscordGuildDto(3000, "Test Guild");
            var userDto = new DiscordUserDto(userDiscordId, "SharedUser", "0");
            var channelDto = new DiscordChannelDto(2000, "general", guildDto);
            var messageDto = new DiscordMessageDto(
                DiscordId: (ulong)(10000 + i),
                Content: $"Message {i}",
                Timestamp: DateTimeOffset.UtcNow,
                Author: userDto,
                Channel: channelDto);

            return EntityManager.GetOrAddMessageAsync(messageDto);
        });

        // Act - Run all in parallel
        await Task.WhenAll(tasks);

        // Assert - Only one user should exist
        await using var db = await GetQueryContextAsync();
        var userCount = await db.Users.CountAsync(u => u.DiscordId == userDiscordId);
        
        userCount.Should().Be(1, "concurrent operations should create only one user entity");
    }

    [Fact]
    public async Task Concurrent_messages_in_same_guild_create_single_guild()
    {
        // Arrange
        const int parallelCount = 10;
        var guildDiscordId = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        
        var tasks = Enumerable.Range(0, parallelCount).Select(i =>
        {
            var guildDto = new DiscordGuildDto(guildDiscordId, "Shared Guild");
            var userDto = new DiscordUserDto((ulong)(4000 + i), $"User{i}", "0");
            var channelDto = new DiscordChannelDto(2000, "general", guildDto);
            var messageDto = new DiscordMessageDto(
                DiscordId: (ulong)(10000 + i),
                Content: $"Message {i}",
                Timestamp: DateTimeOffset.UtcNow,
                Author: userDto,
                Channel: channelDto);

            return EntityManager.GetOrAddMessageAsync(messageDto);
        });

        // Act
        await Task.WhenAll(tasks);

        // Assert
        await using var db = await GetQueryContextAsync();
        var guildCount = await db.Guilds.CountAsync(g => g.DiscordId == guildDiscordId);
        
        guildCount.Should().Be(1, "concurrent operations should create only one guild entity");
    }

    [Fact]
    public async Task Concurrent_messages_in_same_channel_create_single_channel()
    {
        // Arrange
        const int parallelCount = 10;
        var channelDiscordId = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        
        var tasks = Enumerable.Range(0, parallelCount).Select(i =>
        {
            var guildDto = new DiscordGuildDto(3000, "Test Guild");
            var userDto = new DiscordUserDto((ulong)(4000 + i), $"User{i}", "0");
            var channelDto = new DiscordChannelDto(channelDiscordId, "shared-channel", guildDto);
            var messageDto = new DiscordMessageDto(
                DiscordId: (ulong)(10000 + i),
                Content: $"Message {i}",
                Timestamp: DateTimeOffset.UtcNow,
                Author: userDto,
                Channel: channelDto);

            return EntityManager.GetOrAddMessageAsync(messageDto);
        });

        // Act
        await Task.WhenAll(tasks);

        // Assert
        await using var db = await GetQueryContextAsync();
        var channelCount = await db.Channels.CountAsync(c => c.DiscordId == channelDiscordId);
        
        channelCount.Should().Be(1, "concurrent operations should create only one channel entity");
    }

    [Fact]
    public async Task All_parallel_messages_are_persisted()
    {
        // Arrange
        const int parallelCount = 20;
        var baseId = (ulong)Random.Shared.NextInt64(100000, long.MaxValue);
        
        var tasks = Enumerable.Range(0, parallelCount).Select(i =>
        {
            var guildDto = new DiscordGuildDto(baseId, "Test Guild");
            var userDto = new DiscordUserDto(baseId + 1, "User", "0");
            var channelDto = new DiscordChannelDto(baseId + 2, "general", guildDto);
            var messageDto = new DiscordMessageDto(
                DiscordId: baseId + 100 + (ulong)i, // Unique message IDs
                Content: $"Parallel message {i}",
                Timestamp: DateTimeOffset.UtcNow,
                Author: userDto,
                Channel: channelDto);

            return EntityManager.GetOrAddMessageAsync(messageDto);
        });

        // Act
        var results = await Task.WhenAll(tasks);

        // Assert - All messages persisted, no exceptions
        results.Should().HaveCount(parallelCount);
        results.Should().AllSatisfy(m => m.Id.Should().BeGreaterThan(0));

        await using var db = await GetQueryContextAsync();
        var messageCount = await db.Messages.CountAsync();
        messageCount.Should().Be(parallelCount, "all parallel messages should be persisted");
    }
}
