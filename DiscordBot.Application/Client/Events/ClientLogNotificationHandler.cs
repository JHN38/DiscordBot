using DiscordBot.Application.Common;
using DiscordBot.Domain.Client.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Client.Events;

public sealed class ClientLogNotificationHandler(ILogger<ClientLogNotificationHandler> logger) : INotificationHandler<ClientLogNotification>
{
    public Task Handle(ClientLogNotification notification, CancellationToken cancellationToken)
    {
        logger.Log(LogLevelConverter.Convert(notification.Message.Severity), notification.Message.Exception,
            "LOG: {message}", notification.Message.Message ?? notification.Message.Exception?.Message);

        return Task.CompletedTask;
    }
}
