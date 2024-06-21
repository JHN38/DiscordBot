using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Guild.Events;

public record GuildMessageReceivedNotification(SocketMessage Message) : INotification;