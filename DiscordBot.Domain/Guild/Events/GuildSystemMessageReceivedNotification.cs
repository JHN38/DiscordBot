using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Guild.Events;

public record GuildSystemMessageReceivedNotification(SocketSystemMessage Message) : INotification;
