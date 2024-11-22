using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Events;

public record ClientReadyNotification() : INotification;

public class ClientReadyNotificationHandler(ILogger<ClientReadyNotificationHandler> logger,
    DiscordSocketClient client,
    InteractionService commands,
    IMediator mediator) : INotificationHandler<ClientReadyNotification>
{
    private readonly DiscordSocketClient _client = client;
    private readonly IMediator _mediator = mediator;

    public async Task Handle(ClientReadyNotification notification, CancellationToken cancellationToken)
    {
        _client.Log += async (message) => await _mediator.Publish(new ClientLogNotification(message), cancellationToken);
        _client.MessageReceived += async (message) => await _mediator.Publish(new ClientMessageReceivedNotification(message), cancellationToken);
        _client.PresenceUpdated += async (user, oldPresence, newPresence) => await _mediator.Publish(new GuildPresenceUpdateNotification(user, oldPresence, newPresence), cancellationToken);
        _client.GuildScheduledEventCreated += async (guildEvent) => await _mediator.Publish(new GuildScheduledEventNotification(guildEvent), cancellationToken);
        _client.InviteCreated += async (invite) => await _mediator.Publish(new GuildInviteCreatedNotification(invite), cancellationToken);

        // process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += async (socketInteration) => await _mediator.Publish(new ClientInteractionCreatedNotification(socketInteration), cancellationToken);

        // this is where you put the id of the test discord guild
        logger.LogInformation("Bot ready in Debug mode...");

        // process the command execution results 
        commands.SlashCommandExecuted += async (info, context, result)
            => await _mediator.Publish(new SlashCommandExecutedNotification(info, context, result), cancellationToken);

        commands.ContextCommandExecuted += async (info, context, result)
            => await _mediator.Publish(new ContextCommandExecutedNotification(info, context, result), cancellationToken);

        commands.ComponentCommandExecuted += async (info, context, result)
            => await _mediator.Publish(new ComponentCommandExecutedNotification(info, context, result), cancellationToken);

        commands.Log += async (message) => await _mediator.Publish(new CommandLogNotification(message), cancellationToken);

        await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), null);
        await commands.RegisterCommandsGloballyAsync(true);

        logger.LogInformation("Connected as -> [{CurrentUser}] :)", _client.CurrentUser.Username);
    }
}
