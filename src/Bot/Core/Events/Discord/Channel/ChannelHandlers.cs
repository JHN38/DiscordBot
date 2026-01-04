using Discord;
using DiscordBot.Bot.Core.Events.Discord.Client;
using DiscordBot.Contracts.RecordKeeping;
using Wolverine;

namespace DiscordBot.Bot.Core.Events.Discord.Channel;

public sealed class ClientChannelCreatedNotificationHandler(IMessageBus bus)
{
    public async Task Handle(ClientChannelCreatedNotification notification, CancellationToken _)
    {
        if (notification.Channel is not IGuildChannel guildChannel)
            return;

        await bus.PublishAsync(new SaveChannelCommand(
            guildChannel.Id,
            guildChannel.Name,
            guildChannel.GuildId,
            guildChannel.Guild?.Name // Guild might be null if not cached, but IGuildChannel usually has GuildId
        ));
    }
}

public sealed class ClientChannelUpdatedNotificationHandler(IMessageBus bus)
{
    public async Task Handle(ClientChannelUpdatedNotification notification, CancellationToken _)
    {
        if (notification.After is not IGuildChannel guildChannel)
            return;

        await bus.PublishAsync(new SaveChannelCommand(
            guildChannel.Id,
            guildChannel.Name,
            guildChannel.GuildId,
            guildChannel.Guild?.Name
        ));
    }
}
