using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Domain.Entities;
using DiscordBot.Domain.Queries;
using MediatR;

namespace DiscordBot.Application.QueryHandlers;

public class WebSearchRequestHandler(IWebSearchService searchService) : IRequestHandler<WebSearchRequest, WebSearchResponse?>
{
    public Task<WebSearchResponse?> Handle(WebSearchRequest request, CancellationToken cancellationToken)
        => searchService.SearchAsync(request.Query, request.ResultCount, cancellationToken: cancellationToken);
}
