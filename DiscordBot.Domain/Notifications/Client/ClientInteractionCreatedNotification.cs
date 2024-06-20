using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Notifications.Client;

public record ClientInteractionCreatedNotification(SocketInteraction Interaction) : INotification;
