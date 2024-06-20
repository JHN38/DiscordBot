using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Notifications.Client;

public record ClientMessageReceivedNotification(SocketMessage Message) : INotification;
