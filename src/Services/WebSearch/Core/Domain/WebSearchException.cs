namespace DiscordBot.Service.WebSearch.Core.Domain;

/// <summary>
/// Exception thrown when a web search operation fails.
/// </summary>
public class WebSearchException : Exception
{
    public WebSearchException() { }

    public WebSearchException(string message) : base(message) { }

    public WebSearchException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when JSON parsing fails during a web search operation.
/// </summary>
public sealed class WebSearchJsonParseException : WebSearchException
{
    public string? JsonContent { get; }

    public WebSearchJsonParseException(string message) : base(message) { }

    public WebSearchJsonParseException(string message, string? jsonContent) : base(message)
    {
        JsonContent = jsonContent;
    }

    public WebSearchJsonParseException(string message, Exception innerException) : base(message, innerException) { }

    public WebSearchJsonParseException(string message, string? jsonContent, Exception innerException) : base(message, innerException)
    {
        JsonContent = jsonContent;
    }
}
