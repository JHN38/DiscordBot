using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Weather.Commands;

public record ForecastWeatherEmbedRequest(SocketUserMessage Message, string Location) : IRequest;