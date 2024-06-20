using Discord;
using DiscordBot.Domain.Notifications.Client;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.NotificationHandlers.Client;

public class GuildMessageReceivedHandler(ILogger<GuildMessageReceivedHandler> logger) : INotificationHandler<GuildMessageReceivedNotification>
{
    public async Task Handle(GuildMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var msg = message.Content;

        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;

        var author = await guild.GetUserAsync(message.Author.Id);

        foreach (var mentionedUser in message.MentionedUsers)
        {
            var user = await guild.GetUserAsync(mentionedUser.Id);
            msg = msg.Replace(user.Mention, $"@{user.DisplayName}");
        }

        logger.LogInformation("({guild}) #{channel} <{user}> {message}", guild.Name, guildChannel.Name, author.DisplayName, msg);

        // Additional handling logic for guild messages can go here

        await Task.CompletedTask;
    }
}
