using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveVoiceSessionCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SaveVoiceSessionHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveVoiceSessionCommand command, CancellationToken cancellationToken = default)
    {
        var voiceSessionDto = command.ToVoiceSessionDto();
        await entityManager.GetOrAddVoiceSessionAsync(voiceSessionDto, cancellationToken);
    }
}

/// <summary>
/// Handles UpdateVoiceSessionCommand by setting the left timestamp.
/// </summary>
public sealed class UpdateVoiceSessionHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(UpdateVoiceSessionCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        var session = await db.VoiceSessions
            .FirstOrDefaultAsync(v => v.SessionId == command.SessionId, cancellationToken);

        if (session is not null)
        {
            session.LeftAt = command.LeftAt;
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
