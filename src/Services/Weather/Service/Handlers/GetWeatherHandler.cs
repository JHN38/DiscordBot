using DiscordBot.Contracts.Weather;
using DiscordBot.Service.Weather.Core.Domain.Enums;
using DiscordBot.Service.Weather.Core.Interfaces;

using DomainEntities = DiscordBot.Service.Weather.Core.Domain;
using ContractEntities = DiscordBot.Contracts.Weather;

namespace DiscordBot.Service.Weather.Handlers;

public sealed class GetWeatherHandler(IWeatherService weatherService)
{
    public async Task<WeatherResponse?> Handle(GetWeatherQuery query, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<WeatherRequestType>(query.Type, true, out var requestType))
        {
            requestType = WeatherRequestType.Weather;
        }

        var domainResponse = await weatherService.GetWeatherAsync(requestType, query.Location, query.Units, cancellationToken);

        if (domainResponse == null) return null;

        // Map Domain to Contract
        return new WeatherResponse([.. domainResponse.Items.Select(MapItem)]);
    }

    private static WeatherResponseItem MapItem(DomainEntities.WeatherResponseItem item)
    {
        return new WeatherResponseItem(
            new ContractEntities.Location(item.Location.City, item.Location.Country, item.Location.Longitude, item.Location.Latitude, item.Location.Timezone),
            item.Title,
            item.Description,
            item.IconUrl,
            new ContractEntities.Temperature(item.Temperature.Temp, item.Temperature.FeelsLike, item.Temperature.TempMin, item.Temperature.TempMax),
            item.Pressure,
            item.Humidity,
            item.Visibility,
            item.Clouds,
            item.DateTime,
            item.Sunrise,
            item.Sunset,
            new ContractEntities.Wind(item.Wind.Speed, item.Wind.Deg)
        );
    }
}
