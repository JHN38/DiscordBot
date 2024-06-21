using DiscordBot.Application.Common;
using DiscordBot.Domain.Commands.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands.Events;

public sealed class CommandLogNotificationHandler(ILogger<CommandLogNotificationHandler> logger) : INotificationHandler<CommandLogNotification>
{
    public Task Handle(CommandLogNotification notification, CancellationToken cancellationToken)
    {
        logger.Log(LogLevelConverter.Convert(notification.Message.Severity), notification.Message.Exception,
            "CMD LOG: {message}", notification.Message.Message ?? notification.Message.Exception?.Message);

        return Task.CompletedTask;
    }
}
