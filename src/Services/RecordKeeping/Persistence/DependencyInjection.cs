using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using DiscordBot.Service.RecordKeeping.Persistence.Data.Interceptors;
using DiscordBot.Service.RecordKeeping.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Service.RecordKeeping.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddTransient<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        services.AddDbContextFactory<AppDbContext>((s, optionsBuilder) =>
        {
            // Only enable sensitive data logging in development to avoid exposing user data in production logs
            if (environment.IsDevelopment())
            {
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.EnableDetailedErrors();
            }

            optionsBuilder.UseLoggerFactory(s.GetRequiredService<ILoggerFactory>());

            optionsBuilder.ConfigureWarnings(b => b.Log((RelationalEventId.CommandExecuted, LogLevel.Trace)));
            // Lazy loading disabled to prevent N+1 queries - use explicit .Include() for navigation properties
            optionsBuilder.AddInterceptors(s.GetServices<ISaveChangesInterceptor>());

            optionsBuilder.ConfigureWarnings(w => w.Throw(SqlServerEventId.SavepointsDisabledBecauseOfMARS));

            if (configuration.GetConnectionString("SQLite") is { } sqliteConnectionString)
            {
                optionsBuilder.UseSqlite(sqliteConnectionString);
                return;
            }

            if (configuration.GetConnectionString("SqlServer") is { } sqlServerConnectionString)
            {
                optionsBuilder.UseSqlServer(sqlServerConnectionString);
                return;
            }

            throw new InvalidOperationException(
                "No database connection string configured. " +
                "Please configure either 'ConnectionStrings:SQLite' or 'ConnectionStrings:SqlServer' in your application settings.");
        });

        services.AddScoped<AppDbContext>(s => s.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());
        services.AddScoped<IAppDbContext>(s => s.GetRequiredService<AppDbContext>());
        services.AddScoped<IDbInfo, DbInfo>();
        services.AddScoped<IDiscordEntityManager, Services.DiscordEntityManager>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
