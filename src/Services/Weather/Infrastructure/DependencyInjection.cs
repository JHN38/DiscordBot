using DiscordBot.Service.Weather.Core.Interfaces;
using DiscordBot.Service.Weather.Infrastructure.Configuration;
using DiscordBot.Service.Weather.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DiscordBot.Service.Weather.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWeatherInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenWeatherMapConfig>(configuration.GetSection("OpenWeatherApi"));

        services.AddMemoryCache();

        services.AddHttpClient<IWeatherService, OpenWeatherMapService>((s, client) =>
        {
            client.BaseAddress = new Uri(s.GetRequiredService<IOptions<OpenWeatherMapConfig>>().Value.BaseUrl ?? throw new InvalidOperationException("OpenWeatherMap API base URL is not configured"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
