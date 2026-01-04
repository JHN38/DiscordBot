using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using DiscordBot.Service.RecordKeeping.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DiscordBot.Tests.Functional;

public class DiscordEntityManagerTests
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public DiscordEntityManagerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _factory = new TestDbContextFactory(options);
    }

    [Fact]
    public async Task GetOrAddMessageAsync_Persists_Message_And_Dependencies()
    {
        // Arrange
        var manager = new DiscordEntityManager(_factory, NullLogger<DiscordEntityManager>.Instance);

        var guildDto = new DiscordGuildDto(100, "Test Guild");
        var userDto = new DiscordUserDto(200, "Test User", "1234");
        var channelDto = new DiscordChannelDto(300, "general", guildDto);

        // Guild flows through Channel - not passed separately
        var messageDto = new DiscordMessageDto(
            DiscordId: 400,
            Content: "Hello World",
            Timestamp: DateTimeOffset.UtcNow,
            Author: userDto,
            Channel: channelDto);

        // Act
        var result = await manager.GetOrAddMessageAsync(messageDto);

        // Assert
        using var db = await _factory.CreateDbContextAsync();

        var savedMessage = await db.Messages
            .Include(m => m.Author)
            .Include(m => m.Channel)
            .Include(m => m.Guild)
            .FirstOrDefaultAsync(m => m.Id == result.Id);

        Assert.NotNull(savedMessage);
        Assert.Equal(400ul, result.DiscordId);
        Assert.Equal("Hello World", savedMessage.Content);

        Assert.NotNull(savedMessage.Author);
        Assert.Equal(200ul, savedMessage.Author.DiscordId);

        Assert.NotNull(savedMessage.Channel);
        Assert.Equal(300ul, savedMessage.Channel.DiscordId);

        // Guild is derived from Channel
        Assert.NotNull(savedMessage.Guild);
        Assert.Equal(100ul, savedMessage.Guild.DiscordId);
    }

    [Fact]
    public async Task GetOrAddMessageAsync_Reuses_Existing_Dependencies()
    {
        // Arrange
        var manager = new DiscordEntityManager(_factory, NullLogger<DiscordEntityManager>.Instance);

        // Seed existing data
        using (var seedDb = await _factory.CreateDbContextAsync())
        {
            seedDb.Guilds.Add(new DiscordGuild { DiscordId = 100, Name = "Existing Guild" });
            seedDb.Users.Add(new DiscordUser { DiscordId = 200, Username = "ExistingUser", Discriminator = "0000" });
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
        var result = await manager.GetOrAddMessageAsync(messageDto);

        // Assert
        using var db = await _factory.CreateDbContextAsync();

        // Ensure no duplicates
        Assert.Equal(1, await db.Guilds.CountAsync());
        Assert.Equal(1, await db.Users.CountAsync());
        Assert.Equal(1, await db.Channels.CountAsync());

        var savedMessage = await db.Messages
            .Include(m => m.Author)
            .Include(m => m.Channel)
            .Include(m => m.Guild)
            .FirstOrDefaultAsync(m => m.Id == result.Id);

        Assert.NotNull(savedMessage);
        Assert.Equal(100ul, savedMessage.Guild!.DiscordId);
        Assert.Equal("Existing Guild", savedMessage.Guild.Name); // Should preserve existing name
    }

    [Fact]
    public async Task GetOrAddMessageAsync_Returns_Existing_Message()
    {
        // Arrange
        var manager = new DiscordEntityManager(_factory, NullLogger<DiscordEntityManager>.Instance);

        var guildDto = new DiscordGuildDto(100, "Test Guild");
        var userDto = new DiscordUserDto(200, "Test User", "1234");
        var channelDto = new DiscordChannelDto(300, "general", guildDto);

        var messageDto = new DiscordMessageDto(
            DiscordId: 400,
            Content: "Hello World",
            Timestamp: DateTimeOffset.UtcNow,
            Author: userDto,
            Channel: channelDto);

        // Act - Add the same message twice
        var firstResult = await manager.GetOrAddMessageAsync(messageDto);
        var secondResult = await manager.GetOrAddMessageAsync(messageDto);

        // Assert - Should return the same reference
        Assert.Equal(firstResult.Id, secondResult.Id);
        Assert.Equal(firstResult.DiscordId, secondResult.DiscordId);

        // Ensure only one message exists
        using var db = await _factory.CreateDbContextAsync();
        Assert.Equal(1, await db.Messages.CountAsync());
    }

    [Fact]
    public async Task GetOrAddMessageAsync_Handles_DM_Without_Guild()
    {
        // Arrange
        var manager = new DiscordEntityManager(_factory, NullLogger<DiscordEntityManager>.Instance);

        var userDto = new DiscordUserDto(200, "Test User", "1234");
        var channelDto = new DiscordChannelDto(300, "DM Channel"); // No guild for DMs

        var messageDto = new DiscordMessageDto(
            DiscordId: 400,
            Content: "Hello via DM",
            Timestamp: DateTimeOffset.UtcNow,
            Author: userDto,
            Channel: channelDto);

        // Act
        var result = await manager.GetOrAddMessageAsync(messageDto);

        // Assert
        using var db = await _factory.CreateDbContextAsync();

        var savedMessage = await db.Messages
            .Include(m => m.Author)
            .Include(m => m.Channel)
            .Include(m => m.Guild)
            .FirstOrDefaultAsync(m => m.Id == result.Id);

        Assert.NotNull(savedMessage);
        Assert.NotNull(savedMessage.Channel);
        Assert.Null(savedMessage.Guild); // DMs have no guild
        Assert.Equal(0, await db.Guilds.CountAsync()); // No guild was created
    }

    private sealed class TestDbContextFactory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);
        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new AppDbContext(options));
    }
}
