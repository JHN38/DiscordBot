using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Domain.Client.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Discord.Client.Events;

public class ClientInteractionCreatedNotificationHandler(ILogger<ClientInteractionCreatedNotificationHandler> logger,
                                                         DiscordSocketClient client,
                                                         InteractionService commands,
                                                         IServiceProvider services) : INotificationHandler<ClientInteractionCreatedNotification>
{
    public async Task Handle(ClientInteractionCreatedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            // create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            var context = new SocketInteractionContext(client, notification.Interaction);
            switch (notification.Interaction)
            {
                case SocketSlashCommand cmd:
                    logger.LogInformation("INTERACTION: <{user}> ({commandType}) \"/{command} {parameters}\"", cmd.User.Username, cmd.GetType().Name, cmd.CommandName, string.Join(" ", cmd.Data.Options.Select(x => x.Value)));
                    break;
                case SocketUserCommand cmd:
                    logger.LogInformation("INTERACTION: <{user}> ({commandType}) [{member}] => [{command}]", cmd.User.Username, cmd.GetType().Name, cmd.CommandName, cmd.Data.Member.Username);
                    break;
                case SocketMessageCommand cmd:
                    logger.LogInformation("INTERACTION: <{user}> ({commandType}) \"{command} {parameters}\"", cmd.User.Username, cmd.GetType().Name, cmd.CommandName, string.Join(" ", cmd.Data.Options.Select(x => x.Value)));
                    break;
                default: break;
            }

            await commands.ExecuteCommandAsync(context, services);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception happened while trying to run the Interaction.");

            // if a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (notification.Interaction.Type == InteractionType.ApplicationCommand)
            {
                await notification.Interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
