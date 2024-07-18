using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record GuildUserMessageReceivedNotification(IUserMessage Message) : INotification;
