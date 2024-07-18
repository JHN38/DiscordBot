using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Events;

public record GuildScheduledEventNotification(SocketGuildEvent GuildEvent) : INotification;