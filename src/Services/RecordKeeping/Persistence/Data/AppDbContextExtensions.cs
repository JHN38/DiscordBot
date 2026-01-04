using DiscordBot.Service.RecordKeeping.Core.Domain.Common;
using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Persistence.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Service.RecordKeeping.Persistence.Data;

public static class AppDbContextExtensions
{
    private const int MaxRetryAttempts = 3;

    /// <summary>
    /// Gets an entity by Discord ID or creates it if it doesn't exist.
    /// Saves changes immediately after creation to satisfy FK constraints.
    /// Handles race conditions via unique constraint + retry pattern.
    /// </summary>
    public static async Task<TEntity> GetOrCreateByDiscordIdAsync<TEntity>(
        this DbSet<TEntity> dbSet,
        DbContext context,
        ulong discordId,
        Func<TEntity> createEntity,
        CancellationToken cancellationToken = default)
        where TEntity : DiscordEntity
    {
        for (int attempt = 0; attempt < MaxRetryAttempts; attempt++)
        {
            var existing = await dbSet
                .FirstOrDefaultAsync(e => e.DiscordId == discordId, cancellationToken);

            if (existing is not null)
                return existing;

            try
            {
                var created = dbSet.Add(createEntity()).Entity;
                await context.SaveChangesAsync(cancellationToken);
                return created;
            }
            catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
            {
                // Another request beat us to it - detach the failed entity and retry
                var failedEntry = context.ChangeTracker.Entries<TEntity>()
                    .FirstOrDefault(e => e.Entity.DiscordId == discordId);

                if (failedEntry is not null)
                {
                    failedEntry.State = EntityState.Detached;
                }

                // On final attempt, throw
                if (attempt == MaxRetryAttempts - 1)
                {
                    throw new InvalidOperationException(
                        $"Failed to get or create {typeof(TEntity).Name} with DiscordId {discordId} after {MaxRetryAttempts} attempts.", ex);
                }

                // Continue to retry - the entity should now exist
            }
        }

        // Should not reach here due to throw above, but compiler needs this
        throw new InvalidOperationException($"Failed to get or create {typeof(TEntity).Name} with DiscordId {discordId}.");
    }

    /// <summary>
    /// Gets an entity by Discord ID without tracking (read-only).
    /// </summary>
    public static Task<TEntity?> GetByDiscordIdAsync<TEntity>(
        this DbSet<TEntity> dbSet,
        ulong discordId,
        CancellationToken cancellationToken = default)
        where TEntity : DiscordEntity
    {
        return dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.DiscordId == discordId, cancellationToken);
    }
}
