using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record DirectUserMessageReceivedNotification(IUserMessage Message) : INotification;
