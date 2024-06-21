using Discord;
using MediatR;

namespace DiscordBot.Domain.Client.Events;

public record ClientLogNotification(LogMessage Message) : INotification;
