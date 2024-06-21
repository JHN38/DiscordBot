using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.DirectMessage.Events;

public record DirectSystemMessageReceivedNotification(SocketSystemMessage Message) : INotification;
