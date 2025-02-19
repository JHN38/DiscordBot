using DiscordBot.Domain.Entities;
using DiscordBot.Domain.Enums;

namespace DiscordBot.Application.Interfaces;
public interface IWeatherService
{
    Task<WeatherResponse?> GetWeatherAsync(WeatherRequestType requestType, string location, string? units = null, CancellationToken cancellationToken = default);
}