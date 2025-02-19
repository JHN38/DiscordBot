using Discord;
using DiscordBot.Application.Common.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Events;

public record CommandLogNotification(LogMessage Message) : INotification;

public sealed class CommandLogNotificationHandler(ILogger<CommandLogNotificationHandler> logger) : INotificationHandler<CommandLogNotification>
{
    public Task Handle(CommandLogNotification notification, CancellationToken cancellationToken)
    {
        logger.Log(notification.Message.Severity.ConvertToMicrosoft(), notification.Message.Exception,
            "CMD LOG: {Message}", notification.Message.Message ?? notification.Message.Exception?.Message);

        return Task.CompletedTask;
    }
}
