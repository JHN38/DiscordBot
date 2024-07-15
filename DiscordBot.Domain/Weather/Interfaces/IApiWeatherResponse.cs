using DiscordBot.Domain.Weather.Models;

namespace DiscordBot.Domain.Weather.Interfaces;

public interface IApiWeatherResponse
{
    public List<WeatherResponse> ToWeatherResponseList();
}
