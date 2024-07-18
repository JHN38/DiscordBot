using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record CommandLogNotification(LogMessage Message) : INotification;
