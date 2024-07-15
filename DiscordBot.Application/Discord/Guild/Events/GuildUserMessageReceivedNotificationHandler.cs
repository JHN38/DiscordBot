﻿using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using DiscordBot.Application.Common.Helpers;
using DiscordBot.Domain.Commands.Events;
using DiscordBot.Domain.Guild.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Discord.Guild.Events;

public sealed partial class GuildUserMessageReceivedHandler(ILogger<GuildUserMessageReceivedHandler> logger, IMediator mediator) : INotificationHandler<GuildUserMessageReceivedNotification>
{
    public async Task Handle(GuildUserMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;

        var author = await guild.GetUserAsync(message.Author.Id);

        logger.LogInformation("({Guild}) #{Channel} <{User}> {Message}",
            guild.Name, guildChannel.Name, author.DisplayName, await MessageContentHelper.MentionsToText(message));

        if (message.Content.StartsWith('>') && message.Content.Length > 2)
        {
            await mediator.Publish(new TextCommandNotification(message, message.Content[1..]), cancellationToken);
        }

        // @bot mention command
        var botUser = await guildChannel.Guild.GetCurrentUserAsync();
        if (message.MentionedUsers.Any(user => user.Id == botUser.Id) && BotMentionedFirst(message, botUser.Id))
        {
            await mediator.Publish(new MentionCommandNotification(message), cancellationToken);
        }
    }

    public static bool BotMentionedFirst(SocketMessage message, ulong botId)
    {
        var span = message.Content.AsSpan();
        var mention = $"<@{botId}>";
        var mentionWithNickname = $"<@!{botId}>";

        return span.StartsWith(mention.AsSpan(), StringComparison.Ordinal) || span.StartsWith(mentionWithNickname.AsSpan(), StringComparison.Ordinal);
    }
}
