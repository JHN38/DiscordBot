using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Guild.Events;

public record GuildUserMessageReceivedNotification(SocketUserMessage Message) : INotification;
