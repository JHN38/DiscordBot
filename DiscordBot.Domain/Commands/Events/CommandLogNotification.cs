using Discord;
using MediatR;

namespace DiscordBot.Domain.Commands.Events;

public record CommandLogNotification(LogMessage Message) : INotification;
