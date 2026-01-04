using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveScheduledEventCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SaveScheduledEventHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveScheduledEventCommand command, CancellationToken cancellationToken = default)
    {
        var scheduledEventDto = command.ToScheduledEventDto();
        await entityManager.GetOrAddScheduledEventAsync(scheduledEventDto, cancellationToken);
    }
}
