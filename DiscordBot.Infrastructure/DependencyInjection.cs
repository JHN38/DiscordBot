using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Application.Configurations;
using DiscordBot.Application.Discord.Messages;
using DiscordBot.Application.Interfaces;
using DiscordBot.Infrastructure.Configuration;
using DiscordBot.Infrastructure.Data;
using DiscordBot.Infrastructure.Data.Interceptors;
using DiscordBot.Infrastructure.Data.Repositories;
using DiscordBot.Infrastructure.Services;
using Microsoft.EntityFrameworkCore; // Add this using directive
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DiscordBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
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

        services.Configure<BotConfig>(configuration.GetSection("Bot"));
        services.AddSingleton<IBotConfig>(s => s.GetRequiredService<IOptions<BotConfig>>().Value);

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddTransient<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        // Inside the AddInfrastructure method
        services.AddDbContext<AppDbContext>((s, optionsBuilder) =>
        {
            optionsBuilder.UseSqlite(connectionString);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.AddInterceptors(s.GetServices<ISaveChangesInterceptor>());
        });

        services.AddScoped<IAppDbContext>(s => s.GetRequiredService<AppDbContext>());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IDiscordMessageRepository, DiscordMessageRepository>();

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
