using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record DirectMessageReceivedNotification(IMessage Message) : INotification;
