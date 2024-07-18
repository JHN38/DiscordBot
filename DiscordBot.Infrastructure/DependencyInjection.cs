using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Infrastructure.Configuration;
using DiscordBot.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DiscordBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BotConfig>(configuration.GetSection("Bot"));
        services.Configure<GoogleApiConfig>(configuration.GetSection("GoogleApi"));
        services.Configure<OpenWeatherMapConfig>(configuration.GetSection("OpenWeatherApi"));

        services.AddHttpClient<IWebSearchService, GoogleWebSearchService>((s, client) =>
        {
            client.BaseAddress = new Uri(s.GetRequiredService<IOptions<GoogleApiConfig>>().Value.BaseUrl ?? throw new InvalidOperationException("Google API base URL is not configured"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        services.AddHttpClient<IWeatherService, OpenWeatherMapService>((s, client) =>
        {
            client.BaseAddress = new Uri(s.GetRequiredService<IOptions<OpenWeatherMapConfig>>().Value.BaseUrl ?? throw new InvalidOperationException("Google API base URL is not configured"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}