using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Application.Common.Configuration;
using DiscordBot.Domain.Notifications.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot.Application.NotificationHandlers.Client;

public class ClientReadyNotificationHandler(ILogger<ClientReadyNotificationHandler> logger,
    DiscordSocketClient client,
    InteractionService commands,
    IOptions<BotOptions> botOptions) : INotificationHandler<ClientReadyNotification>
{
    public async Task Handle(ClientReadyNotification notification, CancellationToken cancellationToken)
    {
#if DEBUG
        // this is where you put the id of the test discord guild
        logger.LogInformation("Bot ready in Debug mode...");

        if (botOptions.Value.GuildId > 0)
        {
            foreach (var module in commands.Modules)
            {
                foreach (var command in module.SlashCommands)
                {
                    logger.LogDebug("Registering {type} command \"{command}\" from module \"{module}\".", command.CommandType, command.Name, module.Name);
                }
                foreach (var command in module.ContextCommands)
                {
                    logger.LogDebug("Registering {type} command \"{command}\" from module \"{module}\".", command.CommandType, command.Name, module.Name);
                }
            }

            await commands.RegisterCommandsToGuildAsync(botOptions.Value.GuildId, true);
        }
#else
        _logger.LogInformation("Bot ready in Production mode...");
        //await _commands.RegisterCommandsGloballyAsync(true);
#endif

        logger.LogInformation("Connected as -> [{currentUser}] :)", client.CurrentUser.Username);

        //await client.SetGameAsync("with your mind");
    }
}
