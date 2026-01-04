using Discord;
using Discord.Interactions;
using DiscordBot.Bot.Core.Common.Helpers;
using Microsoft.Extensions.Logging;
using static DiscordBot.Bot.Core.Events.Discord.Command.CommandErrorHandler;

namespace DiscordBot.Bot.Core.Events.Discord.Command;

public sealed class CommandLogNotificationHandler(ILogger<CommandLogNotificationHandler> logger)
{
    public Task Handle(Client.InteractionLogNotification notification, CancellationToken _)
    {
        logger.Log(notification.Message.Severity.ConvertToMicrosoft(), notification.Message.Exception,
            "COMMAND: {Message}", notification.Message.Message ?? notification.Message.Exception?.Message);

        return Task.CompletedTask;
    }
}

public sealed class SlashCommandExecutedNotificationHandler(ILogger<SlashCommandExecutedNotificationHandler> logger)
{
    public async Task Handle(InteractionSlashCommandExecutedNotification notification, CancellationToken _)
    {
        if (notification.Result.IsSuccess)
            return;

        await HandleCommandErrorAsync(
            logger,
            notification.Result,
            notification.Context,
            notification.Info.Name);
    }
}

public sealed class ContextCommandExecutedNotificationHandler(ILogger<ContextCommandExecutedNotificationHandler> logger)
{
    public async Task Handle(InteractionContextCommandExecutedNotification notification, CancellationToken _)
    {
        if (notification.Result.IsSuccess)
            return;

        await HandleCommandErrorAsync(
            logger,
            notification.Result,
            notification.Context,
            notification.Info.Name);
    }
}

public sealed class ComponentCommandExecutedNotificationHandler(ILogger<ComponentCommandExecutedNotificationHandler> logger)
{
    public async Task Handle(InteractionComponentCommandExecutedNotification notification, CancellationToken _)
    {
        if (notification.Result.IsSuccess)
            return;

        await HandleCommandErrorAsync(
            logger,
            notification.Result,
            notification.Context,
            notification.Info.Name);
    }
}

internal static class CommandErrorHandler
{
    public static async Task HandleCommandErrorAsync(
        ILogger logger,
        IResult result,
        IInteractionContext context,
        string commandName)
    {
        var errorMessage = result.Error switch
        {
            InteractionCommandError.UnmetPrecondition => $"You don't have permission: {result.ErrorReason}",
            InteractionCommandError.UnknownCommand => "Unknown command.",
            InteractionCommandError.BadArgs => $"Invalid arguments: {result.ErrorReason}",
            InteractionCommandError.Exception => "An error occurred while processing your command.",
            InteractionCommandError.Unsuccessful => $"Command was unsuccessful: {result.ErrorReason}",
            _ => "An unexpected error occurred."
        };

        switch (result.Error)
        {
            case InteractionCommandError.UnmetPrecondition:
                logger.LogWarning("Command '{Command}' precondition not met: {Error}", commandName, result.ErrorReason);
                break;
            case InteractionCommandError.UnknownCommand:
                logger.LogWarning("Unknown command '{Command}' attempted", commandName);
                break;
            case InteractionCommandError.BadArgs:
                logger.LogWarning("Command '{Command}' received bad arguments: {Error}", commandName, result.ErrorReason);
                break;
            case InteractionCommandError.Exception:
                logger.LogError("Command '{Command}' threw an exception: {Error}", commandName, result.ErrorReason);
                break;
            case InteractionCommandError.Unsuccessful:
                logger.LogWarning("Command '{Command}' was unsuccessful: {Error}", commandName, result.ErrorReason);
                break;
            default:
                logger.LogWarning("Command '{Command}' failed with unhandled error type {ErrorType}: {Error}",
                    commandName, result.Error, result.ErrorReason);
                break;
        }

        try
        {
            if (context.Interaction.HasResponded)
            {
                await context.Interaction.FollowupAsync(errorMessage, ephemeral: true);
            }
            else
            {
                await context.Interaction.RespondAsync(errorMessage, ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send error response for command '{Command}'", commandName);
        }
    }
}
