using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.DirectMessage.Events;

public record DirectUserMessageReceivedNotification(SocketUserMessage Message) : INotification;
