using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveBanCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SaveBanHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveBanCommand command, CancellationToken cancellationToken = default)
    {
        var banDto = command.ToBanDto();
        await entityManager.GetOrAddUserBanAsync(banDto, cancellationToken);
    }
}

/// <summary>
/// Handles UpdateBanCommand by marking the ban as inactive (unbanned).
/// Uses atomic ExecuteUpdateAsync for safe concurrent processing.
/// </summary>
public sealed class UpdateBanHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(UpdateBanCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Use subqueries to resolve FKs atomically
        var userIds = db.Users
            .Where(u => u.DiscordId == command.UserId)
            .Select(u => u.Id);

        var guildIds = db.Guilds
            .Where(g => g.DiscordId == command.GuildId)
            .Select(g => g.Id);

        // Find unbanner ID if provided (optional)
        int? unbannedById = null;
        if (command.UnbannedById.HasValue)
        {
            unbannedById = await db.Users
                .Where(u => u.DiscordId == command.UnbannedById.Value)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        // Atomic update: mark all matching active bans as inactive
        await db.UserBans
            .Where(b =>
                userIds.Contains(b.UserId) &&
                guildIds.Contains(b.GuildId) &&
                b.IsActive)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.IsActive, false)
                .SetProperty(b => b.UnbannedAt, command.UnbannedAt)
                .SetProperty(b => b.UnbannedById, unbannedById),
                cancellationToken);
    }
}
