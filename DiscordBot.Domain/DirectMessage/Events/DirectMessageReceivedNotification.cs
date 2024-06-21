using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.DirectMessage.Events;

public record DirectMessageReceivedNotification(SocketMessage Message) : INotification;
