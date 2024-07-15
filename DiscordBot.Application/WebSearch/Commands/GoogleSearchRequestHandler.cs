using System.Text.Json;
using DiscordBot.Application.WebSearch.Interfaces;
using DiscordBot.Domain.WebSearch.Commands;
using DiscordBot.Domain.WebSearch.Models;
using DiscordBot.Domain.WebSearch.Models.Google;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.WebSearch.Commands;

public class GoogleSearchRequestHandler(ILogger<GoogleSearchRequestHandler> logger,
                                           IWebSearchService searchService) : IRequestHandler<WebSearchRequest, WebSearchResult?>
{
    public async Task<WebSearchResult?> Handle(WebSearchRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return null;

        logger.LogDebug("Searching using Google for \"{Query}\"", request.Query);

        var results = await searchService.SearchAsync(request.Query, request.ResultCount);

        if (JsonSerializer.Deserialize<GoogleSearchResult>(results) is not GoogleSearchResult searchResult)
            return null;

        return searchResult.ToWebSearchResult();
    }
}
