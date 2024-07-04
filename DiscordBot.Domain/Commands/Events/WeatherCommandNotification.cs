using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Commands.Events;

public record WeatherCommandNotification(SocketUserMessage Message, string WeatherRequestType, string Location) : INotification;