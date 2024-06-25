using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Commands.Events;
using DiscordBot.Domain.Guild.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Discord.Guild.Events;

public partial class GuildUserMessageReceivedHandler(ILogger<GuildUserMessageReceivedHandler> logger, IMediator mediator) : INotificationHandler<GuildUserMessageReceivedNotification>
{
    private static readonly Regex _searchCommandRegex = SearchCommandRegex();

    public async Task Handle(GuildUserMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var msg = message.Content;

        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;

        var author = await guild.GetUserAsync(message.Author.Id);

        foreach (var mentionedUser in message.MentionedUsers)
        {
            var user = await guild.GetUserAsync(mentionedUser.Id);
            msg = msg.Replace(user.Mention, $"@{user.DisplayName ?? user.GlobalName ?? user.Username}");
        }

        logger.LogInformation("({guild}) #{channel} <{user}> {message}", guild.Name, guildChannel.Name, author.DisplayName, msg);

        var botUser = await guildChannel.Guild.GetCurrentUserAsync();
        if (message.MentionedUsers.Any(user => user.Id == botUser.Id) && BotMentionedFirst(message, botUser.Id))
        {
            await mediator.Publish(new MentionCommandNotification(message), cancellationToken);
        }

        var match = SearchCommandRegex().Match(message.Content);
        if (match.Success)
        {
            var resultCount = match.Groups[1].Value switch
            {
                "" => 1,
                var s when int.TryParse(s, out var n) => Math.Clamp(n, 1, 9),
                _ => 1
            };
            var query = match.Groups[2].Value;

            await mediator.Publish(new SearchCommandNotification(message, query, resultCount), cancellationToken);
        }
    }

    public static bool BotMentionedFirst(SocketMessage message, ulong botId)
    {
        var span = message.Content.AsSpan();
        var mention = $"<@{botId}>";
        var mentionWithNickname = $"<@!{botId}>";

        return span.StartsWith(mention.AsSpan(), StringComparison.Ordinal) || span.StartsWith(mentionWithNickname.AsSpan(), StringComparison.Ordinal);
    }

    [GeneratedRegex("""
        ^>search(\d?)\s+(.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex SearchCommandRegex();
}
