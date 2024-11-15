namespace DiscordBot.Application.Common.Interfaces;

/// <summary>
/// Generic repository interface providing basic CRUD operations for an entity.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface IRepository<T> : IDbContext
    where T : class
{
    Task<T?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(ulong id, CancellationToken cancellationToken = default);
}
