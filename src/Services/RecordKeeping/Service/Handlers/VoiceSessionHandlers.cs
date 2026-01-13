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
/// Uses atomic ExecuteUpdateAsync for safe concurrent processing.
/// </summary>
public sealed class UpdateVoiceSessionHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(UpdateVoiceSessionCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        await db.VoiceSessions
            .Where(v => v.SessionId == command.SessionId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(v => v.LeftAt, command.LeftAt),
                cancellationToken);
    }
}
