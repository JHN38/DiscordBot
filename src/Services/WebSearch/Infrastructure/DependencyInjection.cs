using DiscordBot.Service.WebSearch.Core.Interfaces;
using DiscordBot.Service.WebSearch.Infrastructure.Configuration;
using DiscordBot.Service.WebSearch.Infrastructure.Services.CustomSearchEngine;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DiscordBot.Service.WebSearch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWebSearchInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoogleApiConfig>(configuration.GetSection("GoogleApi"));

        services.AddHttpClient<IWebSearchService, GoogleWebSearchService>((s, client) =>
        {
            client.BaseAddress = new Uri(s.GetRequiredService<IOptions<GoogleApiConfig>>().Value.BaseUrl ?? throw new InvalidOperationException("Google API base URL is not configured"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
