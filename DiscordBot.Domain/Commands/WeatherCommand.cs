using Discord;
using MediatR;

namespace DiscordBot.Domain.Commands;

public record WeatherCommand(IUserMessage Message, string WeatherRequestType, string Location, string? Units = "metric") : IRequest<IUserMessage>;