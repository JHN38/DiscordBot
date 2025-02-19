using DiscordBot.Application.Interfaces;
using DiscordBot.Domain.Entities;
using MediatR;

namespace DiscordBot.Application.Queries;

public record WebSearchRequest(string Query, int? ResultCount = null, string? CountryRestriction = null, string? LanguageRestriction = null) : IRequest<WebSearchResponse>;

public class WebSearchRequestHandler(IWebSearchService searchService) : IRequestHandler<WebSearchRequest, WebSearchResponse?>
{
    public Task<WebSearchResponse?> Handle(WebSearchRequest request, CancellationToken cancellationToken)
        => searchService.SearchAsync(request.Query, request.ResultCount, cancellationToken: cancellationToken);
}
