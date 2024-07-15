using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Commands.Events;

public record SearchCommandNotification(SocketUserMessage Message, string Query, int ResultCount = 1, string? Country = null) : INotification;