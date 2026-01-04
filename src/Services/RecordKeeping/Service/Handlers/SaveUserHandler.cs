using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;

namespace DiscordBot.Service.RecordKeeping.Handlers;

public sealed class SaveUserHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveUserCommand command, CancellationToken cancellationToken)
    {
        var userDto = command.ToUserDto();
        await entityManager.GetOrAddUserAsync(userDto, cancellationToken);
    }
}
