using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveAttachmentCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SaveAttachmentHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveAttachmentCommand command, CancellationToken cancellationToken = default)
    {
        var attachmentDto = command.ToAttachmentDto();
        await entityManager.GetOrAddAttachmentAsync(attachmentDto, cancellationToken);
    }
}
