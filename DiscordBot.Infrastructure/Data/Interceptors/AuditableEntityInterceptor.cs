using DiscordBot.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DiscordBot.Infrastructure.Data.Interceptors;
public class AuditableEntityInterceptor(TimeProvider dateTime) : SaveChangesInterceptor
{
    private readonly TimeProvider _dateTime = dateTime;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is { } context)
            AuditEntities(context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
            AuditEntities(context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void AuditEntities(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            var utcNow = _dateTime.GetUtcNow();
            if (entry.State is EntityState.Added)
                entry.Entity.CreatedOn = utcNow;
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                // Ensure CreatedOn is not modified during an update
                entry.Property(nameof(IAuditableEntity.CreatedOn)).IsModified = false;

                // Update ModifiedOn timestamp
                entry.Entity.ModifiedOn = utcNow;
            }
        }
    }
}

public static class EntityEntryExtensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
