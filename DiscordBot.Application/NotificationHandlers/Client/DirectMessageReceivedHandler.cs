using System;
using Discord;
using DiscordBot.Domain.Notifications.Client;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.NotificationHandlers.Client;

public class DirectMessageReceivedHandler(ILogger<DirectMessageReceivedHandler> logger) : INotificationHandler<DirectMessageReceivedNotification>
{
    public async Task Handle(DirectMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var msg = message.Content;

        var channel = (IDMChannel)message.Channel;

        foreach (var mentionedUser in message.MentionedUsers)
        {
            var user = await channel.GetUserAsync(mentionedUser.Id);
            msg = msg.Replace(user.Mention, $"@{user.GlobalName}");
        }
        
        // Handle direct messages
        logger.LogInformation("DM from <{user}>: {message}", message.Author.GlobalName, msg);

        // Additional handling logic for DMs can go here

        await Task.CompletedTask;
    }
}
