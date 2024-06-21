using DiscordBot.Domain.DirectMessage.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Guild.Events;

public class DirectUserMessageReceivedHandler(ILogger<DirectUserMessageReceivedHandler> logger) : INotificationHandler<DirectUserMessageReceivedNotification>
{
    public async Task Handle(DirectUserMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var msg = message.Content;

        var author = message.Author;

        foreach (var mentionedUser in message.MentionedUsers)
        {
            msg = msg.Replace(mentionedUser.Mention, $"@{mentionedUser.GlobalName ?? mentionedUser.Username}");
        }

        logger.LogInformation("[Direct Message] <{user}> {message}", author.GlobalName, msg);

        await Task.CompletedTask;
    }
}
