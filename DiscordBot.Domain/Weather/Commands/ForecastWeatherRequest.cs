using Discord;
using MediatR;

namespace DiscordBot.Domain.Weather.Commands;

public record ForecastWeatherRequest(string Location) : IRequest<Embed?>;