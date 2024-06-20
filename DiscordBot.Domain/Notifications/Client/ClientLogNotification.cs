using Discord;
using MediatR;

namespace DiscordBot.Domain.Notifications.Client;

public record ClientLogNotification(LogMessage Message) : INotification;
