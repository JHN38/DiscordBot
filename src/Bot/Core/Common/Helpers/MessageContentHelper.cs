using System.Collections.Immutable;
using Discord;

namespace DiscordBot.Bot.Core.Common.Helpers;

public static class MessageContentHelper
{
    public static string? StripUserMention(string content, IUser user)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(user);
        if (content.StartsWith(user.Mention, StringComparison.Ordinal))
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
