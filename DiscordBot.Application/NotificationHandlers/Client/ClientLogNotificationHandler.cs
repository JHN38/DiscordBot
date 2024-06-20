using DiscordBot.Application.Common;
using DiscordBot.Domain.Notifications.Client;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.NotificationHandlers.Client;

public sealed class ClientLogNotificationHandler(ILogger<ClientLogNotificationHandler> logger) : INotificationHandler<ClientLogNotification>
{
    public Task Handle(ClientLogNotification notification, CancellationToken cancellationToken)
    {
        logger.Log(LogLevelConverter.Convert(notification.Message.Severity), notification.Message.Exception,
            "LOG: {message}", notification.Message.Message ?? notification.Message.Exception?.Message);

        return Task.CompletedTask;
    }
}
