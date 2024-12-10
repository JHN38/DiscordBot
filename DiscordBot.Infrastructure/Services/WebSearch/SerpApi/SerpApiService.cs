using DiscordBot.Application.Interfaces;
using DiscordBot.Domain.Entities;
using DiscordBot.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace DiscordBot.Infrastructure.Services.WebSearch.SerpApi;
internal class SerpApiService(HttpClient httpClient, IOptions<GoogleApiConfig> config) : IWebSearchService
{
    public Task<WebSearchResponse?> SearchAsync(string jsonQuery, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WebSearchResponse?> SearchAsync(string query, int? resultCount, string? countryRestriction = null, string? languageRestriction = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> SearchImageAsync(IDictionary<string, string> queryParams)
    {
        throw new NotImplementedException();
    }
}
