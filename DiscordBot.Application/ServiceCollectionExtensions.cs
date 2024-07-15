using System.Reflection;
using ChatGptNet;
using CountryData.Standard;
using DiscordBot.Application.Common.Configuration;
using DiscordBot.Application.WebSearch.Interfaces;
using DiscordBot.Application.WebSearch.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Weather.NET;

namespace DiscordBot.Application;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BotConfig>(configuration.GetSection("Bot"));
        services.Configure<GoogleApiConfig>(configuration.GetSection("GoogleApi"));
        services.Configure<OpenWeatherMapConfig>(configuration.GetSection("OpenWeatherApi"));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddChatGpt(configuration);
        services
            .AddSingleton(s => new WeatherClient(s.GetRequiredService<IOptions<OpenWeatherMapConfig>>().Value.ApiKey))
            .AddSingleton(new CountryHelper())
            .AddSingleton<IWebSearchService, GoogleSearchService>();

        services.AddHttpClient("OpenWeatherMap", (s, client) =>
        {
            var baseAddress = s.GetRequiredService<IOptions<OpenWeatherMapConfig>>().Value.BaseUrl ?? throw new InvalidOperationException("Base URL is not set.");

            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
