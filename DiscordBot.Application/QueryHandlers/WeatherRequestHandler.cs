using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Domain.Entities;
using DiscordBot.Domain.Queries;
using MediatR;

namespace DiscordBot.Application.QueryHandlers;

public class WeatherRequestHandler(IWeatherService weatherService) : IRequestHandler<WeatherRequest, WeatherResponse?>
{
    public Task<WeatherResponse?> Handle(WeatherRequest request, CancellationToken cancellationToken)
        => weatherService.GetWeatherAsync(request.RequestType, request.Location, request.Units, cancellationToken);
}
