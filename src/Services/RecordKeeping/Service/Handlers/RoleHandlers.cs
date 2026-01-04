using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveRoleCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SaveRoleHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveRoleCommand command, CancellationToken cancellationToken = default)
    {
        var roleDto = command.ToRoleDto();
        await entityManager.GetOrAddRoleAsync(roleDto, command.GuildId, cancellationToken);
    }
}

/// <summary>
/// Handles DeleteRoleCommand by soft-deleting the role in the database.
/// </summary>
public sealed class DeleteRoleHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(DeleteRoleCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        var role = await db.Roles
            .FirstOrDefaultAsync(r => r.DiscordId == command.RoleId, cancellationToken);

        if (role is not null)
        {
            role.IsDeleted = true;
            role.DeletedAt = command.DeletedAt;
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
