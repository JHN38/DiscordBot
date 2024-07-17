using DiscordBot.Domain.Weather.Models;

namespace DiscordBot.Domain.Weather.Interfaces;

public interface IApiWeatherResponse
{
    public WeatherResponse ToWeatherResponse();
}
