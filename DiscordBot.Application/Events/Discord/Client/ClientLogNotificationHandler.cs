using Discord;
using DiscordBot.Application.Common.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Events;

public record ClientLogNotification(LogMessage Message) : INotification;

public sealed class ClientLogNotificationHandler(ILogger<ClientLogNotificationHandler> logger) : INotificationHandler<ClientLogNotification>
{
    public Task Handle(ClientLogNotification notification, CancellationToken cancellationToken)
    {
        logger.Log(notification.Message.Severity.ConvertToMicrosoft(), notification.Message.Exception,
            "LOG: {Message}", notification.Message.Message ?? notification.Message.Exception?.Message);

        return Task.CompletedTask;
    }
}
