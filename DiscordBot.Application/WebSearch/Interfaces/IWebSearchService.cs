namespace DiscordBot.Application.WebSearch.Interfaces;

public interface IWebSearchService
{
    /// <summary>
    /// Searches for the specified query using Google Custom Search API.
    /// </summary>
    /// <param name="query">The search query and parameters as a json dictionary.</param>
    /// <returns>A task representing the asynchronous operation, containing search results.</returns>
    Task<string> SearchAsync(string jsonQuery);

    /// <summary>
    /// Asynchronously performs a search with the specified query and optional parameters.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="resultCount">The number of results to return.</param>
    /// <param name="location">Optional country of origin to boost results.</param>
    /// <param name="language">Optional language restriction for documents.</param>
    /// <param name="hqTerms">Optional high-quality terms to append to the query.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the search results as a string.</returns>
    Task<string> SearchAsync(string query, int resultCount, string? location = null, string? language = null, string? hqTerms = null);
}
