using DiscordBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DiscordBot.Infrastructure.Data;
public interface IAppDbContext
{
    DbSet<DiscordChannel> Channels { get; set; }
    DbSet<DiscordGuild> Guilds { get; set; }
    DbSet<DiscordMessage> Messages { get; set; }
    DbSet<DiscordUser> Users { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    DatabaseFacade Database { get; }
}
