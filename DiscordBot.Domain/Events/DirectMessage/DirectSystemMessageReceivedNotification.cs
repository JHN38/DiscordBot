using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record DirectSystemMessageReceivedNotification(ISystemMessage Message) : INotification;
