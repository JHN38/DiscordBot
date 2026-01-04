using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SavePresenceLogCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SavePresenceLogHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SavePresenceLogCommand command, CancellationToken cancellationToken = default)
    {
        var presenceLogDto = command.ToPresenceLogDto();
        await entityManager.GetOrAddPresenceLogAsync(presenceLogDto, cancellationToken);
    }
}
