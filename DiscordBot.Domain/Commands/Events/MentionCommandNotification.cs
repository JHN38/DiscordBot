using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Commands.Events;

public record MentionCommandNotification(SocketUserMessage Message) : INotification;