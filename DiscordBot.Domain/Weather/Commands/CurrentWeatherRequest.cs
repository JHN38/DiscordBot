using Discord;
using MediatR;

namespace DiscordBot.Domain.Weather.Commands;

public record CurrentWeatherRequest(string Location) : IRequest<Embed?>;