using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveMessageCommand by delegating to IDiscordEntityManager.
/// Uses Mapperly source-generated mappers to convert commands to DTOs.
/// </summary>
public sealed class SaveMessageHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveMessageCommand command, CancellationToken cancellationToken = default)
    {
        var messageDto = command.ToMessageDto();
        await entityManager.GetOrAddMessageAsync(messageDto, cancellationToken);
    }
}
