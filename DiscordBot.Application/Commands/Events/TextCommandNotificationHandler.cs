using System.Text.RegularExpressions;
using DiscordBot.Domain.Commands.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands.Events;

public sealed partial class TextCommandNotificationHandler(ILogger<TextCommandNotificationHandler> logger,
                                                   IMediator mediator) : INotificationHandler<TextCommandNotification>
{
    [GeneratedRegex("""
        ^(?<cmd>weather|w)(?<subcmd>f|forecast)?\s+(?<arg>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex WeatherCommandRegex();

    [GeneratedRegex("""
        ^(?<cmd>search|s)(?<count>\d)?(?<cr>\w{2})?\s+(?<arg>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex SearchCommandRegex();

    public async Task Handle(TextCommandNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        logger.LogDebug("Processing text command: {Message}", message.Content);

        if (WeatherCommandRegex().Match(notification.Command) is { Success: true } weatherCommandMatch)
        {
            var subCommand = weatherCommandMatch.Groups["subcmd"].Value;
            var arg = weatherCommandMatch.Groups["arg"].Value;

            var requestType = subCommand;
            var location = arg;

            await mediator.Publish(new WeatherCommandNotification(message, requestType, location), cancellationToken);
        }

        if (SearchCommandRegex().Match(notification.Command) is { Success: true } searchCommandMatch)
        {
            var count = searchCommandMatch.Groups["count"].Value;
            var country = searchCommandMatch.Groups["cr"].Value;
            var arg = searchCommandMatch.Groups["arg"].Value;

            var resultCount = count switch
            {
                var s when int.TryParse(s, out var n) => Math.Clamp(n, 1, 9),
                _ => 1
            };

            await mediator.Publish(new SearchCommandNotification(message, arg, resultCount, country), cancellationToken);
        }

        logger.LogWarning("Unknown command: {Command}", notification.Command);
    }
}
