using System.Text.RegularExpressions;
using Discord;
using DiscordBot.Application.Commands.TextCommands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands;

public record TextCommand(IUserMessage Message, string Command) : IRequest<IUserMessage?>;

public sealed partial class TextCommandHandler(ILogger<TextCommandHandler> logger,
                                                   IMediator mediator) : IRequestHandler<TextCommand, IUserMessage?>
{
    [GeneratedRegex("""
        ^(?<cmd>weather|w)(?<subcmd>forecast|f)?\s+(?<arg>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex WeatherCommandRegex();

    [GeneratedRegex("""
        ^(?<cmd>search|s)(?<count>\d)?(?<cr>\w{2})?\s+(?<arg>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex SearchCommandRegex();

    [GeneratedRegex("""
        ^(?<cmd>msg|m)\s+(?<action>\w+)\s+(?<args>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex MessageCommandRegex();

    [GeneratedRegex("""
        ^(?<cmd>database|db)\s+(?<action>get|g)\s+(?<args>.*)
        """, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex DatabaseCommandRegex();


    public async Task<IUserMessage?> Handle(TextCommand notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        logger.LogDebug("Processing text command: {Message}", message.Content);

        if (WeatherCommandRegex().Match(notification.Command) is { Success: true } weatherCommandMatch)
        {
            var subCommand = weatherCommandMatch.Groups["subcmd"].Value;
            var arg = weatherCommandMatch.Groups["arg"].Value;

            return await mediator.Send(new WeatherCommand(message, subCommand, arg), cancellationToken);
        }

        if (SearchCommandRegex().Match(notification.Command) is { Success: true } searchCommandMatch)
        {
            var country = searchCommandMatch.Groups["cr"].Value;
            var arg = searchCommandMatch.Groups["arg"].Value;
            var resultCount = searchCommandMatch.Groups["count"].Value switch
            {
                var s when int.TryParse(s, out var n) => Math.Clamp(n, 1, 9),
                _ => 1
            };

            return await mediator.Send(new WebSearchCommand(message, arg, resultCount, country), cancellationToken);
        }

        if (MessageCommandRegex().Match(notification.Command) is { Success: true } messageCommandMatch)
        {
            var action = messageCommandMatch.Groups["action"].Value;
            var args = messageCommandMatch.Groups["args"].Value
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            return await mediator.Send(new MessageCommand(message, action, args), cancellationToken);
        }

        if (DatabaseCommandRegex().Match(notification.Command) is { Success: true } databaseCommandMatch)
        {
            var action = databaseCommandMatch.Groups["action"].Value;
            var args = databaseCommandMatch.Groups["args"].Value
                .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            return await mediator.Send(new DatabaseCommand(message, action, args), cancellationToken);
        }

        logger.LogWarning("Unknown command: {Command}", notification.Command);
        return null;
    }
}
