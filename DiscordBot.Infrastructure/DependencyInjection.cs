using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Application.Configurations;
using DiscordBot.Application.Interfaces;
using DiscordBot.Infrastructure.Configuration;
using DiscordBot.Infrastructure.Data;
using DiscordBot.Infrastructure.Data.Interceptors;
using DiscordBot.Infrastructure.Services;
using Microsoft.EntityFrameworkCore; // Add this using directive
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        services.AddDbContext<AppDbContext>((s, options) =>
        {
            options.UseSqlite(connectionString); // This line will now work correctly
            options.EnableSensitiveDataLogging();
            options.UseLoggerFactory(s.GetRequiredService<ILoggerFactory>());
            options.AddInterceptors(s.GetServices<ISaveChangesInterceptor>());
        }, ServiceLifetime.Transient, ServiceLifetime.Transient); // TODO: Change back to Transient when done testing.

        services.AddTransient<IAppDbContext>(s => s.GetRequiredService<AppDbContext>());
        services.AddTransient(typeof(IRepository<>), typeof(RepositoryBase<>));

        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
