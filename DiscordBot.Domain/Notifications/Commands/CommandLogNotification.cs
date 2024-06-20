using Discord;
using MediatR;

namespace DiscordBot.Domain.Notifications.Commands;

public record CommandLogNotification(LogMessage Message) : INotification;
