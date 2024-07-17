using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Weather.Commands;

public record CurrentWeatherEmbedRequest(SocketUserMessage Message, string Location, string? Units = null) : IRequest;