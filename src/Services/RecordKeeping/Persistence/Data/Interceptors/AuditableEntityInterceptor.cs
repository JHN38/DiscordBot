using DiscordBot.Service.RecordKeeping.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DiscordBot.Service.RecordKeeping.Persistence.Data.Interceptors;

public sealed class AuditableEntityInterceptor(TimeProvider dateTime) : SaveChangesInterceptor
{
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

    private void AuditEntities(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            var utcNow = dateTime.GetUtcNow();
            if (entry.State is EntityState.Added)
                entry.Entity.CreatedOn = utcNow;
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Property(nameof(IAuditableEntity.CreatedOn)).IsModified = false;
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
