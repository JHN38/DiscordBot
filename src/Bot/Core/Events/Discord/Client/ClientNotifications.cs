using Discord;
using Discord.WebSocket;

namespace DiscordBot.Bot.Core.Events.Discord.Client;

public record ClientReadyNotification;

public record ClientLogNotification(LogMessage Message);

public record InteractionLogNotification(LogMessage Message);

public record ClientMessageReceivedNotification(IMessage Message);

public record ClientMessageUpdatedNotification(
    Cacheable<IMessage, ulong> Before,
    SocketMessage After,
    ISocketMessageChannel Channel);

public record ClientMessageDeletedNotification(
    Cacheable<IMessage, ulong> Message,
    Cacheable<IMessageChannel, ulong> Channel);

public record ClientMessagesBulkDeletedNotification(
    IReadOnlyCollection<Cacheable<IMessage, ulong>> Messages,
    Cacheable<IMessageChannel, ulong> Channel);

public record ClientReactionAddedNotification(
    Cacheable<IUserMessage, ulong> Message,
    Cacheable<IMessageChannel, ulong> Channel,
    SocketReaction Reaction);

public record ClientReactionRemovedNotification(
    Cacheable<IUserMessage, ulong> Message,
    Cacheable<IMessageChannel, ulong> Channel,
    SocketReaction Reaction);

public record ClientReactionsClearedNotification(
    Cacheable<IUserMessage, ulong> Message,
    Cacheable<IMessageChannel, ulong> Channel);

public record ClientReactionsRemovedForEmoteNotification(
    Cacheable<IUserMessage, ulong> Message,
    Cacheable<IMessageChannel, ulong> Channel,
    IEmote Emote);

public record ClientChannelCreatedNotification(SocketChannel Channel);

public record ClientChannelUpdatedNotification(SocketChannel Before, SocketChannel After);

public record ClientChannelDestroyedNotification(SocketChannel Channel);

public record ClientInteractionCreatedNotification(SocketInteraction Interaction);
