using DiscordBot.Contracts.WebSearch;
using DiscordBot.Service.WebSearch.Core.Interfaces;

using DomainEntities = DiscordBot.Service.WebSearch.Core.Domain;
using ContractEntities = DiscordBot.Contracts.WebSearch;

namespace DiscordBot.Service.WebSearch.Handlers;

public sealed class SearchWebHandler(IWebSearchService webSearchService)
{
    public async Task<WebSearchResponse?> Handle(SearchWebQuery query)
    {
        var domainResponse = await webSearchService.SearchAsync(query.Query, null, null, null);

        if (domainResponse == null) return null;

        // Map Domain to Contract
        return new WebSearchResponse([.. domainResponse.Items.Select(MapItem)]);
    }

    private static ContractEntities.WebSearchResponseItem MapItem(DomainEntities.WebSearchResponseItem item)
    {
        return new ContractEntities.WebSearchResponseItem(
            item.Title,
            item.Link,
            item.DisplayLink,
            item.Snippet,
            [.. item.Thumbnails.Select(t => new ContractEntities.WebSearchImage(t.Src, t.Width, t.Height))]
        );
    }
}
