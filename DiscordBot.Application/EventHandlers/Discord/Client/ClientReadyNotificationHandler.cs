using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.EventHandlers;

public class ClientReadyNotificationHandler(ILogger<ClientReadyNotificationHandler> logger,
    DiscordSocketClient client,
    InteractionService commands,
    IMediator mediator) : INotificationHandler<ClientReadyNotification>
{
    public async Task Handle(ClientReadyNotification notification, CancellationToken cancellationToken)
    {
        // this is where you put the id of the test discord guild
        logger.LogInformation("Bot ready in Debug mode...");

        // process the command execution results 
        commands.SlashCommandExecuted += async (info, context, result)
            => await mediator.Publish(new SlashCommandExecutedNotification(info, context, result), cancellationToken);

        commands.ContextCommandExecuted += async (info, context, result)
            => await mediator.Publish(new ContextCommandExecutedNotification(info, context, result), cancellationToken);

        commands.ComponentCommandExecuted += async (info, context, result)
            => await mediator.Publish(new ComponentCommandExecutedNotification(info, context, result), cancellationToken);

        commands.Log += async (message) => await mediator.Publish(new CommandLogNotification(message), cancellationToken);

        await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), null);
        await commands.RegisterCommandsGloballyAsync(true);

        logger.LogInformation("Connected as -> [{CurrentUser}] :)", client.CurrentUser.Username);
    }
}
