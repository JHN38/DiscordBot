using DiscordBot.Service.WebSearch.Core.Domain;

namespace DiscordBot.Service.WebSearch.Core.Interfaces;

public interface IWebSearchService
{
    Task<WebSearchResponse?> SearchAsync(string jsonQuery, CancellationToken cancellationToken = default);

    Task<WebSearchResponse?> SearchAsync(string query, int? resultCount, string? countryRestriction = null, string? languageRestriction = null, CancellationToken cancellationToken = default);

    Task<string> SearchImageAsync(IDictionary<string, string> queryParams);
}
