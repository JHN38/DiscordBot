using DiscordBot.Application.Common.Helpers;
using DiscordBot.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.EventHandlers;

public sealed class ClientLogNotificationHandler(ILogger<ClientLogNotificationHandler> logger) : INotificationHandler<ClientLogNotification>
{
    public Task Handle(ClientLogNotification notification, CancellationToken cancellationToken)
    {
        logger.Log(notification.Message.Severity.ConvertToMicrosoft(), notification.Message.Exception,
            "LOG: {Message}", notification.Message.Message ?? notification.Message.Exception?.Message);

        return Task.CompletedTask;
    }
}
