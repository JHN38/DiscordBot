using ChatGptNet;
using CountryData.Standard;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly));
        services.AddAutoMapper(AssemblyReference.Assembly);

        services.AddChatGpt(configuration);
        services.AddSingleton(new CountryHelper());

        return services;
    }
}
