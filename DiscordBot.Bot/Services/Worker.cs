using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Application.Events;
using DiscordBot.Infrastructure.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace DiscordBot.Bot.Services;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMediator _mediator;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interaction;
    private readonly IOptions<BotConfig> _botOptions;
    private readonly IServiceScope _serviceScope;

    public Worker(ILogger<Worker> logger,
                DiscordSocketClient client,
                InteractionService interaction,
                IOptions<BotConfig> botOptions,
                IServiceProvider serviceProvider)
    {
        _serviceScope = serviceProvider.CreateScope();
        _mediator = _serviceScope.ServiceProvider.GetRequiredService<IMediator>();
        _logger = logger;
        _client = client;
        _interaction = interaction;
        _botOptions = botOptions;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _client.Ready += async () => await _mediator.Publish(new ClientReadyNotification(), cancellationToken);

            _client.Log += async (message) => await _mediator.Publish(new ClientLogNotification(message), cancellationToken);
            _interaction.Log += async (message) => await _mediator.Publish(new CommandLogNotification(message), cancellationToken);

            _client.MessageReceived += async (message) => await _mediator.Publish(new ClientMessageReceivedNotification(message), cancellationToken);

            // TODO: implement the following events
            //_client.MessageUpdated += async (oldMessage, newMessage, channel) => await _mediator.Publish(new ClientMessageUpdatedNotification(oldMessage, newMessage, channel), cancellationToken);
            //_client.MessageDeleted += async (message, channel) => await _mediator.Publish(new ClientMessageDeletedNotification(message, channel), cancellationToken);
            //_client.MessagesBulkDeleted += async (messages, channel) => await _mediator.Publish(new ClientMessagesBulkDeletedNotification(messages, channel), cancellationToken);

            //_client.ReactionAdded += async (message, channel, reaction) => await _mediator.Publish(new ClientReactionAddedNotification(message, channel, reaction), cancellationToken);
            //_client.ReactionRemoved += async (message, channel, reaction) => await _mediator.Publish(new ClientReactionRemovedNotification(message, channel, reaction), cancellationToken);
            //_client.ReactionsCleared += async (message, channel) => await _mediator.Publish(new ClientReactionsClearedNotification(message, channel), cancellationToken);
            //_client.ReactionsRemovedForEmote += async (message, channel, emote) => await _mediator.Publish(new ClientReactionsRemovedForEmoteNotification(message, channel, emote), cancellationToken);

            //_client.ChannelCreated += async (channel) => await _mediator.Publish(new ClientChannelCreatedNotification(channel), cancellationToken);
            //_client.ChannelUpdated += async (oldChannel, newChannel) => await _mediator.Publish(new ClientChannelUpdatedNotification(oldChannel, newChannel), cancellationToken);
            //_client.ChannelDestroyed += async (channel) => await _mediator.Publish(new ClientChannelDestroyedNotification(channel), cancellationToken);

            //_client.GuildAvailable += async (guild) => await _mediator.Publish(new GuildAvailableNotification(guild), cancellationToken);
            //_client.GuildUnavailable += async (guild) => await _mediator.Publish(new GuildUnavailableNotification(guild), cancellationToken);
            //_client.GuildUpdated += async (oldGuild, newGuild) => await _mediator.Publish(new GuildUpdatedNotification(oldGuild, newGuild), cancellationToken);

            //_client.RoleCreated += async (role) => await _mediator.Publish(new GuildRoleCreatedNotification(role), cancellationToken);
            //_client.RoleUpdated += async (oldRole, newRole) => await _mediator.Publish(new GuildRoleUpdatedNotification(oldRole, newRole), cancellationToken);
            //_client.RoleDeleted += async (role) => await _mediator.Publish(new GuildRoleDeletedNotification(role), cancellationToken);

            //_client.UserUpdated += async (oldUser, newUser) => await _mediator.Publish(new GuildUserUpdatedNotification(oldUser, newUser), cancellationToken);
            //_client.UserVoiceStateUpdated += async (user, oldVoiceState, newVoiceState) => await _mediator.Publish(new GuildUserVoiceStateUpdatedNotification(user, oldVoiceState, newVoiceState), cancellationToken);
            //_client.UserIsTyping += async (user, channel) => await _mediator.Publish(new GuildUserIsTypingNotification(user, channel), cancellationToken);

            //_client.GuildMemberUpdated += async (oldMember, newMember) => await _mediator.Publish(new GuildMemberUpdatedNotification(oldMember, newMember), cancellationToken);
            //_client.GuildMembersDownloaded += async (guild) => await _mediator.Publish(new GuildMembersDownloadedNotification(guild), cancellationToken);

            //_client.UserJoined += async (user) => await _mediator.Publish(new GuildUserJoinedNotification(user), cancellationToken);
            //_client.UserLeft += async (guild, user) => await _mediator.Publish(new GuildUserLeftNotification(guild, user), cancellationToken);
            //_client.UserBanned += async (user, guild) => await _mediator.Publish(new GuildUserBannedNotification(user, guild), cancellationToken);
            //_client.UserUnbanned += async (user, guild) => await _mediator.Publish(new GuildUserUnbannedNotification(user, guild), cancellationToken);

            _client.PresenceUpdated += async (user, oldPresence, newPresence) => await _mediator.Publish(new GuildPresenceUpdateNotification(user, oldPresence, newPresence), cancellationToken);
            _client.GuildScheduledEventCreated += async (guildEvent) => await _mediator.Publish(new GuildScheduledEventNotification(guildEvent), cancellationToken);
            _client.InviteCreated += async (invite) => await _mediator.Publish(new GuildInviteCreatedNotification(invite), cancellationToken);

            // process the InteractionCreated payloads to execute Interactions commands
            _client.InteractionCreated += async (socketInteration) => await _mediator.Publish(new ClientInteractionCreatedNotification(socketInteration), cancellationToken);

            // process the command execution results 
            _interaction.SlashCommandExecuted += async (info, context, result)
                => await _mediator.Publish(new SlashCommandExecutedNotification(info, context, result), cancellationToken);
            _interaction.ContextCommandExecuted += async (info, context, result)
                => await _mediator.Publish(new ContextCommandExecutedNotification(info, context, result), cancellationToken);
            _interaction.ComponentCommandExecuted += async (info, context, result)
                => await _mediator.Publish(new ComponentCommandExecutedNotification(info, context, result), cancellationToken);

            await _client.LoginAsync(TokenType.Bot, _botOptions.Value.Token);
            await _client.StartAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An Exception occured while starting the service.");
        }

        _logger.LogInformation("Hosted Service started.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Service stopping.");
        _serviceScope.Dispose();

        return Task.CompletedTask;
    }
}
