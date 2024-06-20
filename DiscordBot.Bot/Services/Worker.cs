using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Application.Common.Configuration;
using DiscordBot.Domain.Notifications.Client;
using DiscordBot.Domain.Notifications.Commands;
using MediatR;
using Microsoft.Extensions.Options;

namespace DiscordBot.Bot.Services;

public class Worker(ILogger<Worker> logger,
                    DiscordSocketClient client,
                    IOptions<BotOptions> botOptions,
                    InteractionService commands,
                    IServiceProvider services,
                    IMediator mediator) : IHostedService
{
    public async Task StartAsync(CancellationToken stoppingToken)
    {
        try
        {
            client.Log += async (message) => await mediator.Publish(new CommandLogNotification(message), stoppingToken);
            commands.Log += async (message) => await mediator.Publish(new CommandLogNotification(message), stoppingToken);

            client.MessageReceived += async (message) => await mediator.Publish(new ClientMessageReceivedNotification(message), stoppingToken);
            client.Ready += async () => await mediator.Publish(new ClientReadyNotification(), stoppingToken);

            await client.LoginAsync(TokenType.Bot, botOptions.Value.Token);
            await client.StartAsync();

            await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);

            // process the InteractionCreated payloads to execute Interactions commands
            client.InteractionCreated += async (socketInteration) => await mediator.Publish(new ClientInteractionCreatedNotification(socketInteration), stoppingToken);

            // process the command execution results 
            commands.SlashCommandExecuted += async (info, context, result)
                => await mediator.Publish(new SlashCommandExecutedNotification(info, context, result), stoppingToken);

            commands.ContextCommandExecuted += async (info, context, result)
                => await mediator.Publish(new ContextCommandExecutedNotification(info, context, result), stoppingToken);

            commands.ComponentCommandExecuted += async (info, context, result)
                => await mediator.Publish(new ComponentCommandExecutedNotification(info, context, result), stoppingToken);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occured while starting the service.");
        }

        logger.LogInformation("Hosted Service started.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Hosted Service stopping.");

        return Task.CompletedTask;
    }
}
