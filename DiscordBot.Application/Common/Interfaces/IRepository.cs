using DiscordBot.Domain.Common;

namespace DiscordBot.Application.Common.Interfaces;

/// <summary>
/// Base repository interface providing basic CRUD operations for an entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being managed by the repository.</typeparam>
public interface IRepositoryBase<TEntity> : IDbContext
    where TEntity : class
{
    /// <summary>
    /// Retrieves all entities of the specified type asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity to the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for entities with generic method-level identifiers.
/// Extends basic CRUD functionality to support identifier-based retrieval and conditional operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being managed by the repository.</typeparam>
public interface IRepository<TEntity> : IRepositoryBase<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Retrieves an entity by its identifier asynchronously.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identifier.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The entity if found, or null if not found.</returns>
    Task<TEntity?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : struct;

    /// <summary>
    /// Retrieves an entity by its identifier or creates a new one if it does not exist.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identifier.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="createEntity">A function to create a new entity if it does not exist.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The existing or newly created entity.</returns>
    Task<TEntity> GetOrCreateEntityAsync<TId>(TId id, Func<TEntity> createEntity, CancellationToken cancellationToken = default) where TId : struct;

    /// <summary>
    /// Adds a new entity if it does not already exist in the repository.
    /// </summary>
    /// <typeparam name="TId">The type of the entity identifier.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddIfNewAsync<TId>(TId id, TEntity entity, CancellationToken cancellationToken = default) where TId : struct, IEquatable<TId>;
}

/// <summary>
/// Repository interface for entities with a strongly typed identifier.
/// Extends base CRUD functionality with identifier-specific operations.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
/// <typeparam name="TEntity">The type of the entity being managed by the repository.</typeparam>
public interface IRepository<TId, TEntity> : IRepositoryBase<TEntity>
    where TId : struct, IEquatable<TId>
    where TEntity : EntityBase<TId>
{
    /// <summary>
    /// Retrieves an entity by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The entity if found, or null if not found.</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an entity by its identifier or creates a new one if it does not exist.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="createEntity">A function to create a new entity if it does not exist.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The existing or newly created entity.</returns>
    Task<TEntity> GetOrCreateEntityAsync(TId id, Func<TEntity> createEntity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity if it does not already exist in the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddIfNewAsync(TEntity entity, CancellationToken cancellationToken = default);
}
