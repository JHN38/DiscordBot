using Discord;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace DiscordBot.Bot.Core.Commands.TextCommands;

public record MessageCommand(IUserMessage Message, string Action, IEnumerable<string>? Args = null);

#pragma warning disable CS9113 // Parameter 'bus' is unread - will be used when RecordKeeping queries are implemented
public sealed class MessageCommandHandler(ILogger<MessageCommandHandler> logger, IMessageBus bus)
#pragma warning restore CS9113
{
    public async Task<IUserMessage> Handle(MessageCommand command, CancellationToken cancellationToken)
    {
        var message = command.Message;
        var channel = message.Channel;

        var action = command.Action;
        var args = command.Args;

        using var typingState = channel.EnterTypingState();

        try
        {
            return action switch
            {
                "g" or "get" => await GetMessagesAsync(message, args, cancellationToken),
                _ => await HandleUnknownActionAsync(message, action, cancellationToken)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the message command {Action}.", action);
            return await message.ReplyAsync("Sorry, I couldn't process your request at the moment.", options: new() { CancelToken = cancellationToken });
        }
    }

    private async Task<IUserMessage> HandleUnknownActionAsync(IUserMessage message, string action, CancellationToken cancellationToken)
    {
        logger.LogWarning("Unknown subcommand: {Action}", action);
        return await message.ReplyAsync($"Unknown subcommand: {action}", options: new() { CancelToken = cancellationToken });
    }

    private async Task<IUserMessage> GetMessagesAsync(IUserMessage message, IEnumerable<string>? args, CancellationToken cancellationToken = default) =>
        args?.FirstOrDefault() switch
        {
            "last" => await GetLastMessagesAsync(message, args.ElementAtOrDefault(1), cancellationToken),
            "first" => await GetFirstMessagesAsync(message, args.ElementAtOrDefault(1), cancellationToken),
            "find" => await GetMatchingMessagesAsync(message, args.ElementAtOrDefault(1), cancellationToken),
            _ => await GetLastMessagesAsync(message, null, cancellationToken)
        };

    private async Task<IUserMessage> GetMatchingMessagesAsync(IUserMessage message, string? needle, CancellationToken cancellationToken)
    {
        if (needle is null)
            return await message.ReplyAsync("No search query given.",
                options: new() { CancelToken = cancellationToken });

        // Query RecordKeeping service via Wolverine (not yet implemented)
        return await message.ReplyAsync("Message search is being migrated to use the RecordKeeping service.",
            options: new() { CancelToken = cancellationToken });
    }

    private async Task<IUserMessage> GetFirstMessagesAsync(IUserMessage message, string? numberOfMessagesString, CancellationToken cancellationToken)
    {
        _ = int.TryParse(numberOfMessagesString, out _);

        return await message.ReplyAsync("Query RecordKeeping service (not yet implemented)",
            options: new() { CancelToken = cancellationToken });
    }

    private async Task<IUserMessage> GetLastMessagesAsync(IUserMessage message, string? numberOfMessagesString, CancellationToken cancellationToken = default)
    {
        _ = int.TryParse(numberOfMessagesString, out _);

        return await message.ReplyAsync("Query RecordKeeping service (not yet implemented)",
            options: new() { CancelToken = cancellationToken });
    }
}
