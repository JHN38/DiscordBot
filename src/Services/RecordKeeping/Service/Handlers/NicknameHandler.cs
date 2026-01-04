using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveNicknameCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SaveNicknameHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveNicknameCommand command, CancellationToken cancellationToken = default)
    {
        var nicknameDto = command.ToNicknameDto();
        await entityManager.GetOrAddUserNicknameAsync(nicknameDto, cancellationToken);
    }
}
