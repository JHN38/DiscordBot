using DiscordBot.Domain.Entities;
using DiscordBot.Domain.Enums;
using MediatR;

namespace DiscordBot.Domain.Queries;

public partial record WeatherRequest(WeatherRequestType RequestType, string Location, string? Units = null) : IRequest<WeatherResponse?>;
