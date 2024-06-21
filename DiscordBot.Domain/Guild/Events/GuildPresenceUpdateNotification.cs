using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Guild.Events;

public record GuildPresenceUpdateNotification(SocketUser User, SocketPresence OldPresence, SocketPresence NewPresence) : INotification;
