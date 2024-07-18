using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record ClientLogNotification(LogMessage Message) : INotification;
