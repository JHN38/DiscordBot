using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record ClientMessageReceivedNotification(IMessage Message) : INotification;
