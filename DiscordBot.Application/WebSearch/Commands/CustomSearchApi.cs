using DiscordBot.Application.Common.Configuration;
using DiscordBot.Domain.WebSearch.Commands;
using DiscordBot.Domain.WebSearch.Models;
using Google.Apis.CustomSearchAPI.v1;
using Google.Apis.CustomSearchAPI.v1.Data;
using Google.Apis.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace DiscordBot.Application.WebSearch.Commands;

public class CustomSearchApiRequestHandler(ILogger<CustomSearchApiRequestHandler> logger,
                                           IOptions<GoogleApiOptions> googleApi) : IRequestHandler<WebSearchRequest, IEnumerable<WebSearchResponse>>
{
    public async Task<IEnumerable<WebSearchResponse>> Handle(WebSearchRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return [];

        var initializer = new BaseClientService.Initializer
        {
            ApiKey = googleApi.Value.ApiKey
        };

        var customSearchService = new CustomSearchAPIService(initializer);
        var listRequest = customSearchService.Cse.List();
        listRequest.Cx = googleApi.Value.SearchEngineId;
        listRequest.Q = request.Query;
        listRequest.Num = Math.Clamp(request.ResultCount, 1, googleApi.Value.MaxResultCount);
        listRequest.Sort = "date";

        if (await listRequest.ExecuteAsync(cancellationToken) is not Search search)
            return [];

        var results = new List<WebSearchResponse>();
        foreach (var item in search.Items)
        {
            logger.LogDebug("Title: {Title}, Link: {Link}, Snippet: {Snippet}", item.Title, item.Link, item.Snippet);

            string? thumbnailUrl = null;
            if (item.Pagemap?.ContainsKey("cse_thumbnail") == true && ((JArray)item.Pagemap["cse_thumbnail"])?.FirstOrDefault() is JToken cseThumbnail)
            {
                thumbnailUrl = cseThumbnail["src"]?.ToString();
            }

            results.Add(new WebSearchResponse(item.Title, item.Link, item.Snippet, thumbnailUrl));
        }

        return results;
    }
}
