using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Commands.Events;

public record TextCommandNotification(SocketUserMessage Message, string Command) : INotification;