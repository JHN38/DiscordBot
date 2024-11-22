using System.Globalization;
using System.Threading;
using Discord;
using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Application.Discord.Messages;
using DiscordBot.Domain.Common.Extensions;
using DiscordBot.Domain.Entities;
using DiscordBot.Infrastructure.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Infrastructure.Data.Repositories;

public class DiscordMessageRepository(AppDbContext context,
                                      IRepository<DiscordGuild> guildRepository,
                                      IRepository<DiscordChannel> channelRepository,
                                      IRepository<DiscordUser> userRepository)
    : Repository<ulong, DiscordMessage>(context), IDiscordMessageRepository
{
    private readonly IRepository<DiscordGuild> _guildRepository = guildRepository;
    private readonly IRepository<DiscordChannel> _channelRepository = channelRepository;
    private readonly IRepository<DiscordUser> _userRepository = userRepository;

    public async Task AddWithDependenciesAsync(IMessage message, IChannel channel, IGuild guild, IUser author, CancellationToken cancellationToken = default)
    {
        // Start a transaction
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var guildEntity = await _guildRepository.GetOrCreateEntityAsync(guild.Id, guild.ToDiscordGuild, cancellationToken);
            var userEntity = await _userRepository.GetOrCreateEntityAsync(author.Id, author.ToDiscordUser, cancellationToken);
            var channelEntity = await _channelRepository.GetOrCreateEntityAsync(channel.Id, channel.ToDiscordChannel, cancellationToken);

            if (!guildEntity.Users.Contains(userEntity))
            {
                guildEntity.Users.Add(userEntity);
            }

            if (!channelEntity.Users.Contains(userEntity))
            {
                channelEntity.Users.Add(userEntity);
            }

            await SaveChangesAsync(cancellationToken);

            var discordMessage = message.ToDiscordMessage();
            await AddAsync(discordMessage, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            // Rollback transaction on error
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetFirstMessagesAsync(int numberOfMessages, CancellationToken cancellationToken = default)
    {
        var messages = await _dbSet
            .Join(_context.Users,                           // Second table to join (Users)
                  m => m.AuthorId,      // Foreign key in Messages (AuthorId)
                  u => u.Id,              // Primary key in Users (Id)
                  (m, u) => new // Projection
                  {
                      u.Username,
                      m.Content,
                      m.Timestamp
                  })
            .OrderBy(m => m.Timestamp)  // Order by Timestamp descending
            .Take(numberOfMessages)
            .ToListAsync(cancellationToken);

        return messages.Select(static m =>
            $"{m.Timestamp.ToString("M/dd/yyyy h:mm tt", CultureInfo.InvariantCulture)} **{m.Username}** {m.Content}");
    }

    public async Task<IEnumerable<string>> GetLastMessagesAsync(int numberOfMessages, CancellationToken cancellationToken = default)
    {
        var messages = await _dbSet
            .Join(_context.Users,                           // Second table to join (Users)
                  m => m.AuthorId,      // Foreign key in Messages (AuthorId)
                  u => u.Id,              // Primary key in Users (Id)
                  (m, u) => new // Projection
                  {
                      u.Username,
                      m.Content,
                      m.Timestamp
                  })
            .OrderByDescending(m => m.Timestamp)  // Order by Timestamp descending
            .Take(numberOfMessages)
            .ToListAsync(cancellationToken);

        return messages.Select(static m =>
            $"{m.Timestamp.ToString("M/dd/yyyy h:mm tt", CultureInfo.InvariantCulture)} **{m.Username}** {m.Content}");
    }

    /// <summary>
    /// Retrieves Discord messages containing the specified term with their associated channels populated.
    /// </summary>
    /// <param name="needle">The term to search for in the message content.</param>
    /// <param name="cancellationToken">Token for cancelling the operation.</param>
    /// <returns>A collection of DiscordMessage entities with their associated channels populated.</returns>
    public async Task<IEnumerable<DiscordMessage>> GetMatchingMessagesAsync(string needle, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(needle))
        {
            throw new ArgumentException("Search term must not be null or empty", nameof(needle));
        }

        var messages = await _dbSet
            .Where(m => EF.Functions.Like(m.Content, $"%{needle}%"))
            .Include(m => m.Channel)
            .Include(m => m.Author)
            .ToListAsync(cancellationToken);

        return messages;
    }
}
