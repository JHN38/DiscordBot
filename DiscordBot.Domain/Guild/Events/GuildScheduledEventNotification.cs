using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Guild.Events;

public record GuildScheduledEventNotification(SocketGuildEvent GuildEvent) : INotification;