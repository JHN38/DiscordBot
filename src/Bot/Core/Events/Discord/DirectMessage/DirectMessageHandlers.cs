using Discord;
using DiscordBot.Bot.Core.Events.Discord.Client;
using DiscordBot.Contracts.RecordKeeping;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace DiscordBot.Bot.Core.Events.Discord.DirectMessage;

public sealed class DirectUserMessageReceivedNotificationHandler(
    ILogger<DirectUserMessageReceivedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(ClientMessageReceivedNotification notification, CancellationToken _)
    {
        if (notification.Message is not IUserMessage message ||
            message.Channel is not IDMChannel ||
            message.Author.IsBot)
            return;

        logger.LogInformation("DM: <{User}> {Message}",
            message.Author.Username, message.Content);

        // Save DM to RecordKeeping service (no guild)
        await bus.PublishAsync(new SaveMessageCommand(
            message.Id,
            message.Channel.Id,
            message.Channel.Name,
            GuildId: null,
            GuildName: null,
            message.Author.Id,
            message.Author.Username,
            message.Content,
            message.Timestamp));
    }
}

public sealed class DirectSystemMessageReceivedNotificationHandler(ILogger<DirectSystemMessageReceivedNotificationHandler> logger)
{
    public Task Handle(ClientMessageReceivedNotification notification, CancellationToken _)
    {
        if (notification.Message is not ISystemMessage message ||
            message.Channel is not IDMChannel)
            return Task.CompletedTask;

        logger.LogInformation("DM SYSTEM: {Message}", message.Content);

        return Task.CompletedTask;
    }
}
