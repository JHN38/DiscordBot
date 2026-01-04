using DiscordBot.Service.WebSearch.Core.Domain;
using DiscordBot.Service.WebSearch.Core.Interfaces;

namespace DiscordBot.Service.WebSearch.Infrastructure.Services.SerpApi;

internal sealed class SerpApiService : IWebSearchService
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
