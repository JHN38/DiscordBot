using Discord;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace DiscordBot.Bot.Core.Commands.TextCommands;

public record DatabaseCommand(IUserMessage Message, string Action, IEnumerable<string>? Args = null);

#pragma warning disable CS9113 // Parameter 'bus' is unread - will be used when RecordKeeping queries are implemented
public sealed class DatabaseCommandHandler(ILogger<DatabaseCommandHandler> logger, IMessageBus bus)
#pragma warning restore CS9113
{
    public async Task<IUserMessage> Handle(DatabaseCommand command, CancellationToken cancellationToken)
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
                "g" or "get" => await GetActionAsync(message, action, args, cancellationToken),
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

    private async Task<IUserMessage> GetActionAsync(IUserMessage message, string action, IEnumerable<string>? args, CancellationToken cancellationToken = default) =>
        args?.FirstOrDefault() switch
        {
            "schema" => await GetDatabaseSchemaAsync(message, cancellationToken),
            _ => await message.ReplyAsync($"Unknown argument for sub-command \"{action}\"", options: new() { CancelToken = cancellationToken })
        };

    private async Task<IUserMessage> GetDatabaseSchemaAsync(IUserMessage message, CancellationToken cancellationToken)
    {
        // Query RecordKeeping service via Wolverine (not yet implemented)
        return await message.ReplyAsync("Database schema query is being migrated to use the RecordKeeping service.",
            options: new() { CancelToken = cancellationToken });
    }
}
