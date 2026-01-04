using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveInviteCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SaveInviteHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveInviteCommand command, CancellationToken cancellationToken = default)
    {
        var inviteDto = command.ToInviteDto();
        await entityManager.GetOrAddInviteAsync(inviteDto, cancellationToken);
    }
}

/// <summary>
/// Handles DeleteInviteCommand by revoking the invite (soft delete).
/// </summary>
public sealed class DeleteInviteHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(DeleteInviteCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        var invite = await db.Invites
            .FirstOrDefaultAsync(i => i.Code == command.Code, cancellationToken);

        if (invite is not null)
        {
            invite.IsRevoked = true;
            invite.RevokedAt = command.RevokedAt;
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
