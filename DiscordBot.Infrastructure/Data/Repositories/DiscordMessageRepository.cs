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
        var discordMessage = message.ToDiscordMessage();

        // Start a transaction
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Try to insert the message
            await AddAsync(discordMessage, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            // Commit transaction
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.IsForeignKeyViolation())
        {
            // Clear the changes to start over if a dependency was missing
            _context.ChangeTracker.Clear();

            // Save entities in order respecting foreign keys
            var discordGuild = await _guildRepository.GetOrCreateEntityAsync(guild.Id, guild.ToDiscordGuild, cancellationToken);
            await _guildRepository.AddIfNewAsync(discordGuild.Id, discordGuild, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            var discordChannel = await _channelRepository.GetOrCreateEntityAsync(channel.Id, channel.ToDiscordChannel, cancellationToken);
            await _channelRepository.AddIfNewAsync(discordChannel.Id, discordChannel, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            // Most likely the user was missing
            var discordUser = await _userRepository.GetOrCreateEntityAsync(author.Id, author.ToDiscordUser, cancellationToken);
            await _userRepository.AddIfNewAsync(author.Id, discordUser, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            // Everything should have been created now
            await AddAsync(discordMessage, cancellationToken);
            await SaveChangesAsync(cancellationToken);

            // Commit transaction
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

    public async Task<IEnumerable<DiscordMessage>> GetMatchingMessagesAsync(string needle, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => EF.Functions.Like(m.Content, $"%{needle}%"))
            .Join(_context.Users,                      // Second table to join (Users)
                  m => m.AuthorId,                     // Foreign key in Messages (AuthorId)
                  u => u.Id,                           // Primary key in Users (Id)
                  (m, u) => m)
            .OrderByDescending(m => m.Timestamp)        // Order by Timestamp descending
            .ToListAsync(cancellationToken);
    }
}
