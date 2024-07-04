using System.Reflection;
using ChatGptNet;
using CountryData.Standard;
using DiscordBot.Application.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Weather.NET;

namespace DiscordBot.Application;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BotOptions>(configuration.GetSection("Bot"));
        services.Configure<GoogleApiOptions>(configuration.GetSection("GoogleApi"));
        services.Configure<WeatherOptions>(configuration.GetSection("OpenWeatherApi"));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddChatGpt(configuration);
        services
            .AddSingleton((s) => new WeatherClient(s.GetRequiredService<IOptions<WeatherOptions>>().Value.ApiKey))
            .AddSingleton(new CountryHelper());

        services.AddHttpClient("OpenWeatherMap", client =>
        {
            client.BaseAddress = new Uri("https://api.openweathermap.org/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
