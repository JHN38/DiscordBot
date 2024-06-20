using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Notifications.Client;

public record DirectMessageReceivedNotification(SocketMessage Message) : INotification;
