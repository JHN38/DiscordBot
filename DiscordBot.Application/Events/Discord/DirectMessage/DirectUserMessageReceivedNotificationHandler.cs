using Discord;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Events;

public record DirectUserMessageReceivedNotification(IUserMessage Message) : INotification;

public class DirectUserMessageReceivedHandler(ILogger<DirectUserMessageReceivedHandler> logger) : INotificationHandler<DirectUserMessageReceivedNotification>
{
    public async Task Handle(DirectUserMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var author = message.Author;

        logger.LogInformation("[Direct Message] <{User}> {Message}", author.GlobalName ?? author.Username, message.CleanContent);

        await Task.CompletedTask;
    }
}
