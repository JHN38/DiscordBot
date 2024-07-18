using System.Collections.Immutable;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Application.Common.Helpers;

public static class MessageContentHelper
{
    public static async Task<string> MentionsToText(IUserMessage message)
    {
        var msg = message.Content;

        var channel = (IGuildChannel)message.Channel;

        foreach (var mentionedUserId in message.MentionedUserIds)
        {
            var user = await channel.GetUserAsync(mentionedUserId);
            msg = msg.Replace(user.Mention, $"@{user.DisplayName ?? user.GlobalName ?? user.Username}");
        }

        return msg;
    }
    public static string? StripBotMention(string content)
    {
        var span = content.AsSpan();
        content = span[(span.IndexOf(' ') + 1)..].ToString();

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
