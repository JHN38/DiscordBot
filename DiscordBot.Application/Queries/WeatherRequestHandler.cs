using DiscordBot.Application.Interfaces;
using DiscordBot.Domain.Entities;
using DiscordBot.Domain.Enums;
using MediatR;

namespace DiscordBot.Application.Queries;

public partial record WeatherRequest(WeatherRequestType RequestType, string Location, string? Units = null) : IRequest<WeatherResponse?>;


public class WeatherRequestHandler(IWeatherService weatherService) : IRequestHandler<WeatherRequest, WeatherResponse?>
{
    public Task<WeatherResponse?> Handle(WeatherRequest request, CancellationToken cancellationToken)
        => weatherService.GetWeatherAsync(request.RequestType, request.Location, request.Units, cancellationToken);
}
