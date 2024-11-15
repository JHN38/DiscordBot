using Discord;
using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Infrastructure.Data;

/// <summary>
/// Provides a generic repository implementation for CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public class RepositoryBase<TEntity>(IAppDbContext context) : IRepository<TEntity>
    where TEntity : class
{
    protected readonly IAppDbContext _context = context;
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task<TEntity?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default) =>
        await _dbSet.FindAsync([id], cancellationToken);

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.ToListAsync(cancellationToken);

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _dbSet.Add(entity).Context.SaveChangesAsync(cancellationToken);

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        _dbSet.Update(entity).Context.SaveChangesAsync(cancellationToken);

    public async Task DeleteAsync(ulong id, CancellationToken cancellationToken = default)
    {
        if (await GetByIdAsync(id, cancellationToken) is { } entity)
            await _dbSet.Remove(entity).Context.SaveChangesAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
