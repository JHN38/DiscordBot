using System.Reflection;
using ChatGptNet;
using DiscordBot.Application.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Application;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BotOptions>(configuration.GetSection("Bot"));
        services.Configure<GoogleApiOptions>(configuration.GetSection("GoogleApi"));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        // Adds ChatGPT service using settings from IConfiguration.
        services.AddChatGpt(configuration);

        return services;
    }
}
