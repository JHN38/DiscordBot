using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record GuildSystemMessageReceivedNotification(ISystemMessage Message) : INotification;
