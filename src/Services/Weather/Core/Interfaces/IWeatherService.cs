using DiscordBot.Service.Weather.Core.Domain;
using DiscordBot.Service.Weather.Core.Domain.Enums;

namespace DiscordBot.Service.Weather.Core.Interfaces;

public interface IWeatherService
{
    Task<WeatherResponse?> GetWeatherAsync(WeatherRequestType requestType, string location, string? units = null, CancellationToken cancellationToken = default);
}
