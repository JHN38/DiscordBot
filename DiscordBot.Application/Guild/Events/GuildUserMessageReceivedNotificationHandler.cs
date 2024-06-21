using Discord;
using Discord.WebSocket;
using DiscordBot.Domain.Commands.Events;
using DiscordBot.Domain.Guild.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Guild.Events;

public class GuildUserMessageReceivedHandler(ILogger<GuildUserMessageReceivedHandler> logger, IMediator mediator) : INotificationHandler<GuildUserMessageReceivedNotification>
{
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

        await Task.CompletedTask;
    }

    public static bool BotMentionedFirst(SocketMessage message, ulong botId)
    {
        var span = message.Content.AsSpan();
        var mention = $"<@{botId}>";
        var mentionWithNickname = $"<@!{botId}>";

        return span.StartsWith(mention.AsSpan(), StringComparison.Ordinal) || span.StartsWith(mentionWithNickname.AsSpan(), StringComparison.Ordinal);
    }

    public static string? StripMention(string content)
    {
        var span = content.AsSpan();

        return span[(span.IndexOf(' ') + 1)..].ToString();
    }
}
