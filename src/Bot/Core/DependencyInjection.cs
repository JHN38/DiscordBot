using CountryData.Standard;
using DiscordBot.Bot.Core.Interfaces;
using DiscordBot.Bot.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Bot.Core;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBotCore(this IServiceCollection services)
    {
        services.AddSingleton(new CountryHelper());
        services.AddTransient<IDiscordUserDisplayNameResolver, DiscordUserDisplayNameResolver>();

        return services;
    }
}
