using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Client.Events;

public record ClientMessageReceivedNotification(SocketMessage Message) : INotification;
