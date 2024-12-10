using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Events;

public record ClientReadyNotification() : INotification;

/// <summary>
/// The event is triggered every time the bot is ready.
/// 
/// This also means every time the bot gets reconnected even if the program was not terminated.
/// </summary>
/// <param name="logger"></param>
/// <param name="client"></param>
/// <param name="interaction"></param>
public class ClientReadyNotificationHandler(ILogger<ClientReadyNotificationHandler> logger,
    DiscordSocketClient client,
    InteractionService interaction) : INotificationHandler<ClientReadyNotification>
{
    private readonly DiscordSocketClient _client = client;
    private readonly InteractionService _interaction = interaction;

    public async Task Handle(ClientReadyNotification notification, CancellationToken cancellationToken)
    {
        await _interaction.AddModulesAsync(Assembly.GetExecutingAssembly(), null);
        await _interaction.RegisterCommandsGloballyAsync(true);

        logger.LogInformation("Connected as -> [{CurrentUser}] :)", _client.CurrentUser.Username);
    }
}
