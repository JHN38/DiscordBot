using System;
using Discord;
using DiscordBot.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands.TextCommands;

public record DatabaseCommand(IUserMessage Message, string Action, IEnumerable<string>? Args = null) : IRequest<IUserMessage>;

public class DatabaseCommandHandler(ILogger<DatabaseCommandHandler> logger, IDbInfo dbInfo) : IRequestHandler<DatabaseCommand, IUserMessage>
{
    private readonly IDbInfo _dbInfo = dbInfo;
    private IUserMessage? _message = null!;

    public async Task<IUserMessage> Handle(DatabaseCommand notification, CancellationToken cancellationToken)
    {
        _message = notification.Message;
        var channel = _message.Channel;

        var action = notification.Action;
        var args = notification.Args;

        // Start typing...
        using var typingState = channel.EnterTypingState();

        try
        {
            switch (action)
            {
                case "g":
                case "get": return await GetActionAsync(action, args, cancellationToken);
                default:
                    logger.LogWarning("Unknown subcommand: {Action}", action);
                    return await _message.ReplyAsync($"Unknown subcommand: {action}", options: new() { CancelToken = cancellationToken });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the message command {Action}.", action);
            return await _message.ReplyAsync("Sorry, I couldn't process your request at the moment.", options: new() { CancelToken = cancellationToken });
        }
    }

    private async Task<IUserMessage> GetActionAsync(string action, IEnumerable<string>? args, CancellationToken cancellationToken = default) =>
        // "last 2"
        args?.FirstOrDefault() switch
        {
            "schema" => await GetDatabaseSchemaAsync(cancellationToken),
            _ => await _message.ReplyAsync($"Unknown argument for sub-command \"{action}\"", options: new() { CancelToken = cancellationToken })
        };

    private async Task<IUserMessage> GetDatabaseSchemaAsync(CancellationToken cancellationToken)
    {
        var schema = await _dbInfo.GetDatabaseSchemaAsync(cancellationToken);
        return await _message.ReplyAsync($"""
            Current database schema:
            ```json
            {schema}
            ```
            """, options: new() { CancelToken = cancellationToken });
    }
}


