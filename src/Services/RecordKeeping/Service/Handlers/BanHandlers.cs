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
/// </summary>
public sealed class UpdateBanHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(UpdateBanCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Find the user and guild first
        var userId = await db.Users
            .Where(u => u.DiscordId == command.UserId)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var guildId = await db.Guilds
            .Where(g => g.DiscordId == command.GuildId)
            .Select(g => (int?)g.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!userId.HasValue || !guildId.HasValue)
            return;

        // Find the active ban and mark it as inactive
        var ban = await db.UserBans
            .Where(b => b.UserId == userId.Value && b.GuildId == guildId.Value && b.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (ban is not null)
        {
            ban.IsActive = false;
            ban.UnbannedAt = command.UnbannedAt;

            // Optionally resolve the unbanner
            if (command.UnbannedById.HasValue)
            {
                var unbannedById = await db.Users
                    .Where(u => u.DiscordId == command.UnbannedById.Value)
                    .Select(u => (int?)u.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (unbannedById.HasValue)
                    ban.UnbannedById = unbannedById.Value;
            }

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
