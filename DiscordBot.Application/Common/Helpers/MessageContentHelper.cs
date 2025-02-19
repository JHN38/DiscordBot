using System.Collections.Immutable;
using Discord;

namespace DiscordBot.Application.Common.Helpers;

public static class MessageContentHelper
{
    /// <summary>
    /// Strips the bot mention from the start of the content, if present.
    /// </summary>
    /// <param name="content">The content string potentially containing the bot mention.</param>
    /// <param name="user">The user whose mention to strip.</param>
    /// <returns>
    /// The content string without the bot mention at the start, or null if the resulting string is null or whitespace.
    /// </returns>
    public static string? StripUserMention(string content, IUser user)
    {
        if (content.StartsWith(user.Mention))
        {
            content = content[user.Mention.Length..].TrimStart();
        }

        return string.IsNullOrWhiteSpace(content) ? null : content;
    }

    public static ImmutableList<string> SplitResponseIntoChunks(string response, int maxChunkSize = 2000)
    {
        var result = new List<string>();
        var span = response.AsSpan();

        while (span.Length > maxChunkSize)
        {
            var splitIndex = span[..maxChunkSize].LastIndexOf(' ');

            if (splitIndex == -1)
            {
                splitIndex = maxChunkSize;
            }

            result.Add(span[..splitIndex].ToString());
            span = span[splitIndex..].TrimStart();
        }

        if (span.Length > 0)
        {
            result.Add(span.ToString());
        }

        return [.. result];
    }
}
