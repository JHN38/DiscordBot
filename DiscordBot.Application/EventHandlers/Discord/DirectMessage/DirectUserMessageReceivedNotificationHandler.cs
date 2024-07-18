using DiscordBot.Application.Common.Helpers;
using DiscordBot.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.EventHandlers;

public class DirectUserMessageReceivedHandler(ILogger<DirectUserMessageReceivedHandler> logger) : INotificationHandler<DirectUserMessageReceivedNotification>
{
    public async Task Handle(DirectUserMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var author = message.Author;

        logger.LogInformation("[Direct Message] <{User}> {Message}", author.GlobalName ?? author.Username, await MessageContentHelper.MentionsToText(message));

        await Task.CompletedTask;
    }
}
