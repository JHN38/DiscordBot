using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;

namespace DiscordBot.Service.RecordKeeping.Handlers;

public sealed class SaveChannelHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveChannelCommand command, CancellationToken cancellationToken)
    {
        var channelDto = command.ToChannelDto();
        await entityManager.GetOrAddChannelAsync(channelDto, cancellationToken);
    }
}
