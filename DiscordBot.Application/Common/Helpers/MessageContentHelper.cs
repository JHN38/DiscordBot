using Discord;
using Discord.WebSocket;

namespace DiscordBot.Application.Common.Helpers;

internal class MessageContentHelper
{
    public static async Task<string> MentionsToText(SocketUserMessage message)
    {
        var msg = message.Content;

        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;

        foreach (var mentionedUser in message.MentionedUsers)
        {
            var user = await guild.GetUserAsync(mentionedUser.Id);
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

    public static List<string> SplitResponseIntoChunks(string response, int maxChunkSize = 2000)
    {
        var result = new List<string>();
        var span = response.AsSpan();

        while (span.Length > maxChunkSize)
        {
            var splitIndex = span.Slice(0, maxChunkSize).LastIndexOf(' ');

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

        return result;
    }
}
