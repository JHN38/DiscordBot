using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveStickerCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SaveStickerHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveStickerCommand command, CancellationToken cancellationToken = default)
    {
        var stickerDto = command.ToStickerDto();
        await entityManager.GetOrAddStickerAsync(stickerDto, cancellationToken);
    }
}
