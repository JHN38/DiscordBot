using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Bot.Core.Common.Helpers;
using DiscordBot.Contracts.RecordKeeping;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace DiscordBot.Bot.Core.Events.Discord.Client;

public sealed class ClientReadyNotificationHandler(
    ILogger<ClientReadyNotificationHandler> logger,
    DiscordSocketClient client,
    InteractionService interaction,
    IServiceProvider serviceProvider)
{
    public async Task Handle(ClientReadyNotification _, CancellationToken __)
    {
        await interaction.AddModulesAsync(Assembly.GetExecutingAssembly(), serviceProvider);
        await interaction.RegisterCommandsGloballyAsync(true);

        logger.LogInformation("Connected as -> [{CurrentUser}] :)", client.CurrentUser.Username);
    }
}

public sealed class ClientLogNotificationHandler(ILogger<ClientLogNotificationHandler> logger)
{
    public Task Handle(ClientLogNotification notification, CancellationToken _)
    {
        logger.Log(notification.Message.Severity.ConvertToMicrosoft(), notification.Message.Exception,
            "LOG: {Message}", notification.Message.Message ?? notification.Message.Exception?.Message);

        return Task.CompletedTask;
    }
}

public sealed class ClientMessageReceivedNotificationHandler(ILogger<ClientMessageReceivedNotificationHandler> logger)
{
    public Task Handle(ClientMessageReceivedNotification _, CancellationToken __)
    {
        try
        {
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occurred in ClientMessageReceivedNotificationHandler.");
            return Task.CompletedTask;
        }
    }
}

public sealed class ClientInteractionCreatedNotificationHandler(
    ILogger<ClientInteractionCreatedNotificationHandler> logger,
    DiscordSocketClient client,
    InteractionService commands,
    IServiceProvider services)
{
    public async Task Handle(ClientInteractionCreatedNotification notification, CancellationToken _)
    {
        try
        {
            var context = new SocketInteractionContext(client, notification.Interaction);
            switch (notification.Interaction)
            {
                case SocketSlashCommand cmd:
                    logger.LogInformation("INTERACTION: <{User}> ({CommandType}) \"/{Command} {Parameters}\"",
                        cmd.User.Username, cmd.GetType().Name, cmd.CommandName,
                        string.Join(" ", cmd.Data.Options.Select(x => x.Value)));
                    break;
                case SocketUserCommand cmd:
                    logger.LogInformation("INTERACTION: <{User}> ({CommandType}) [{Member}] => [{Command}]",
                        cmd.User.Username, cmd.GetType().Name, cmd.CommandName, cmd.Data.Member.Username);
                    break;
                case SocketMessageCommand cmd:
                    logger.LogInformation("INTERACTION: <{User}> ({CommandType}) \"{Command} {Parameters}\"",
                        cmd.User.Username, cmd.GetType().Name, cmd.CommandName,
                        string.Join(" ", cmd.Data.Options.Select(x => x.Value)));
                    break;
                default: break;
            }

            await commands.ExecuteCommandAsync(context, services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception happened while trying to run the Interaction.");

            if (notification.Interaction.Type == InteractionType.ApplicationCommand)
            {
                var msg = await notification.Interaction.GetOriginalResponseAsync();
                await msg.DeleteAsync();
            }
        }
    }
}

public sealed class ClientReactionAddedNotificationHandler(
    ILogger<ClientReactionAddedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(ClientReactionAddedNotification notification, CancellationToken _)
    {
        var reaction = notification.Reaction;

        logger.LogDebug("REACTION ADDED: {User} reacted with {Emote} to message {MessageId}",
            reaction.User.IsSpecified ? reaction.User.Value.Username : "Unknown",
            reaction.Emote.Name,
            reaction.MessageId);

        if (!reaction.User.IsSpecified)
            return;

        await bus.PublishAsync(new SaveReactionCommand(
            reaction.MessageId,
            reaction.UserId,
            reaction.Emote.Name,
            EmojiId: reaction.Emote is Emote customEmote ? customEmote.Id : null,
            IsCustomEmoji: reaction.Emote is Emote,
            IsAnimated: reaction.Emote is Emote emote && emote.Animated,
            IsBurst: false));
    }
}

public sealed class ClientReactionRemovedNotificationHandler(
    ILogger<ClientReactionRemovedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(ClientReactionRemovedNotification notification, CancellationToken _)
    {
        var reaction = notification.Reaction;

        logger.LogDebug("REACTION REMOVED: {User} removed {Emote} from message {MessageId}",
            reaction.User.IsSpecified ? reaction.User.Value.Username : "Unknown",
            reaction.Emote.Name,
            reaction.MessageId);

        if (!reaction.User.IsSpecified)
            return;

        await bus.PublishAsync(new RemoveReactionCommand(
            reaction.MessageId,
            reaction.UserId,
            reaction.Emote.Name,
            EmojiId: reaction.Emote is Emote customEmote ? customEmote.Id : null,
            RemovedAt: DateTimeOffset.UtcNow));
    }
}

public sealed class ClientReactionsClearedNotificationHandler(
    ILogger<ClientReactionsClearedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(ClientReactionsClearedNotification notification, CancellationToken _)
    {
        logger.LogDebug("REACTIONS CLEARED: All reactions cleared from message {MessageId}",
            notification.Message.Id);

        await bus.PublishAsync(new ClearReactionsCommand(
            notification.Message.Id,
            DateTimeOffset.UtcNow));
    }
}

public sealed class ClientReactionsRemovedForEmoteNotificationHandler(
    ILogger<ClientReactionsRemovedForEmoteNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(ClientReactionsRemovedForEmoteNotification notification, CancellationToken _)
    {
        logger.LogDebug("REACTIONS REMOVED FOR EMOTE: {Emote} removed from message {MessageId}",
            notification.Emote.Name,
            notification.Message.Id);

        await bus.PublishAsync(new RemoveEmoteReactionsCommand(
            notification.Message.Id,
            notification.Emote.Name,
            EmojiId: notification.Emote is Emote customEmote ? customEmote.Id : null,
            RemovedAt: DateTimeOffset.UtcNow));
    }
}

public sealed class ClientMessageDeletedNotificationHandler(
    ILogger<ClientMessageDeletedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(ClientMessageDeletedNotification notification, CancellationToken _)
    {
        logger.LogDebug("MESSAGE DELETED: Message {MessageId} deleted",
            notification.Message.Id);

        await bus.PublishAsync(new DeleteMessageCommand(
            notification.Message.Id,
            DateTimeOffset.UtcNow));
    }
}

public sealed class ClientMessagesBulkDeletedNotificationHandler(
    ILogger<ClientMessagesBulkDeletedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(ClientMessagesBulkDeletedNotification notification, CancellationToken _)
    {
        var messageIds = notification.Messages.Select(m => m.Id).ToArray();

        logger.LogInformation("MESSAGES BULK DELETED: {Count} messages deleted",
            messageIds.Length);

        await bus.PublishAsync(new BulkDeleteMessagesCommand(
            messageIds,
            DateTimeOffset.UtcNow));
    }
}

public sealed class ClientMessageUpdatedNotificationHandler(
    ILogger<ClientMessageUpdatedNotificationHandler> logger,
    IMessageBus bus)
{
    public async Task Handle(ClientMessageUpdatedNotification notification, CancellationToken _)
    {
        var message = notification.After;

        if (message.Author?.IsBot == true)
            return;

        logger.LogDebug("MESSAGE UPDATED: Message {MessageId} updated",
            message.Id);

        await bus.PublishAsync(new UpdateMessageCommand(
            message.Id,
            message.Content,
            message.EditedTimestamp ?? DateTimeOffset.UtcNow));
    }
}
