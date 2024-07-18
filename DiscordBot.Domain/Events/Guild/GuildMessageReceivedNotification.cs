using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record GuildMessageReceivedNotification(IMessage Message) : INotification;