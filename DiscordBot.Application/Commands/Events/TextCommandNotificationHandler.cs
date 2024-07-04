using System.Text.RegularExpressions;
using DiscordBot.Domain.Commands.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands.Events;

public sealed partial class TextCommandNotificationHandler(ILogger<TextCommandNotificationHandler> logger,
                                                   IMediator mediator) : INotificationHandler<TextCommandNotification>
{
    public async Task Handle(TextCommandNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        logger.LogDebug("Processing text command: {Message}", message.Content);

        switch (notification.Command)
        {
            case "s" or "search":
                var resultCount = notification.SubCommand switch
                {
                    var s when int.TryParse(s, out var n) => Math.Clamp(n, 1, 9),
                    _ => 1
                };

                await mediator.Publish(new SearchCommandNotification(message, notification.Arg, resultCount), cancellationToken);
                break;
            case "w" or "weather":
                var requestType = notification.SubCommand;
                var location = notification.Arg;

                await mediator.Publish(new WeatherCommandNotification(message, requestType, location), cancellationToken);
                break;

            default:
                logger.LogWarning("Unknown command: {Command}", notification.Command);
                break;
        }
    }
}
