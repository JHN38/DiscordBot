using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Infrastructure.Data;

/// <summary>
/// Provides a generic repository implementation for CRUD operations,
/// leveraging Entity Framework DbSet for database interactions.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being managed by the repository.</typeparam>
public abstract class RepositoryBase<TEntity>(AppDbContext context) : IRepositoryBase<TEntity>
    where TEntity : class
{
    /// <summary>
    /// The database context used for accessing and managing data.
    /// </summary>
    protected readonly AppDbContext _context = context;

    /// <summary>
    /// The DbSet representing the entity type managed by this repository.
    /// </summary>
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.ToListAsync(cancellationToken);

    /// <inheritdoc />
    public Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        Task.FromResult(_dbSet.Add(entity).Entity);

    /// <inheritdoc />
    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        Task.FromResult(_dbSet.Update(entity).Entity);

    /// <inheritdoc />
    public Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        Task.FromResult(_dbSet.Remove(entity).Entity);

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}

/// <summary>
/// Provides a repository implementation for entities with a strongly typed identifier.
/// Implements CRUD operations and identifier-based retrieval.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity being managed by the repository.</typeparam>
public class Repository<TEntity>(AppDbContext context) : RepositoryBase<TEntity>(context), IRepository<TEntity>
    where TEntity : class
{
    /// <inheritdoc />
    public Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : struct =>
        _dbSet.FindAsync([id], cancellationToken).AsTask();

    /// <inheritdoc />
    public async Task<TEntity> GetOrCreateEntityAsync<TId>(TId id, Func<TEntity> createEntity,
        CancellationToken cancellationToken = default) where TId : struct =>
        await GetByIdAsync(id, cancellationToken) ?? await AddAsync(createEntity(), cancellationToken);

    /// <inheritdoc />
    public async Task AddIfNewAsync<TId>(TId id, TEntity entity, CancellationToken cancellationToken = default)
        where TId : struct, IEquatable<TId>
    {
        if (await _dbSet.FindAsync([id], cancellationToken) is null)
            await _dbSet.AddAsync(entity, cancellationToken);
    }
}

/// <summary>
/// Provides a repository implementation for entities with a strongly typed identifier.
/// Implements CRUD operations and identifier-based retrieval.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity being managed by the repository.</typeparam>
public class Repository<TId, TEntity>(AppDbContext context) : RepositoryBase<TEntity>(context), IRepository<TId, TEntity>
    where TId : struct, IEquatable<TId>
    where TEntity : EntityBase<TId>
{
    /// <inheritdoc />
    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default) =>
        _dbSet.FindAsync([id], cancellationToken).AsTask();

    /// <inheritdoc />
    public async Task<TEntity> GetOrCreateEntityAsync(TId id, Func<TEntity> createEntity,
        CancellationToken cancellationToken = default) =>
        await GetByIdAsync(id, cancellationToken) ?? createEntity();

    /// <inheritdoc />
    public async Task<TEntity> GetOrCreateEntityAsync(TId id, Func<Task<TEntity>> createEntityAsync,
        CancellationToken cancellationToken = default) =>
        await GetByIdAsync(id, cancellationToken) ?? await createEntityAsync();

    /// <inheritdoc />
    public async Task AddIfNewAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (!await _dbSet.AnyAsync(e => e.Id.Equals(entity.Id), cancellationToken))
            await _dbSet.AddAsync(entity, cancellationToken);
    }
}
