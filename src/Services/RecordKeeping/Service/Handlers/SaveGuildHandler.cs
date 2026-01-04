using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveGuildCommand by delegating to IDiscordEntityManager.
/// Uses Mapperly source-generated mappers to convert commands to DTOs.
/// </summary>
public sealed class SaveGuildHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveGuildCommand command, CancellationToken cancellationToken = default)
    {
        var guildDto = command.ToGuildDto();
        await entityManager.GetOrAddGuildAsync(guildDto, cancellationToken);
    }
}
