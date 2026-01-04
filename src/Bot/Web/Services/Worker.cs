using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Bot.Core.Events.Discord.Client;
using DiscordBot.Bot.Core.Events.Discord.Command;
using DiscordBot.Bot.Core.Events.Discord.Guild;
using DiscordBot.Bot.Web.Configuration;
using Microsoft.Extensions.Options;
using Wolverine;

namespace DiscordBot.Bot.Web.Services;

internal sealed class Worker(
    ILogger<Worker> logger,
    DiscordSocketClient client,
    InteractionService interaction,
    IOptions<BotConfig> botOptions,
    IServiceProvider serviceProvider,
    IHostApplicationLifetime applicationLifetime) : IHostedService
{
    private CancellationTokenSource? _cts;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // Register event handlers immediately
        client.Ready += OnClientReady;
        client.Log += OnClientLog;
        interaction.Log += OnInteractionLog;
        client.MessageReceived += OnMessageReceived;
        client.MessageUpdated += OnMessageUpdated;
        client.MessageDeleted += OnMessageDeleted;
        client.MessagesBulkDeleted += OnMessagesBulkDeleted;
        client.ReactionAdded += OnReactionAdded;
        client.ReactionRemoved += OnReactionRemoved;
        client.ReactionsCleared += OnReactionsCleared;
        client.ReactionsRemovedForEmote += OnReactionsRemovedForEmote;
        client.ChannelCreated += OnChannelCreated;
        client.ChannelUpdated += OnChannelUpdated;
        client.ChannelDestroyed += OnChannelDestroyed;
        client.GuildAvailable += OnGuildAvailable;
        client.GuildUnavailable += OnGuildUnavailable;
        client.GuildUpdated += OnGuildUpdated;
        client.RoleCreated += OnRoleCreated;
        client.RoleUpdated += OnRoleUpdated;
        client.RoleDeleted += OnRoleDeleted;
        client.UserUpdated += OnUserUpdated;
        client.UserVoiceStateUpdated += OnUserVoiceStateUpdated;
        client.UserIsTyping += OnUserIsTyping;
        client.GuildMemberUpdated += OnGuildMemberUpdated;
        client.GuildMembersDownloaded += OnGuildMembersDownloaded;
        client.UserJoined += OnUserJoined;
        client.UserLeft += OnUserLeft;
        client.UserBanned += OnUserBanned;
        client.UserUnbanned += OnUserUnbanned;
        client.PresenceUpdated += OnPresenceUpdated;
        client.GuildScheduledEventCreated += OnGuildScheduledEventCreated;
        client.InviteCreated += OnInviteCreated;
        client.InteractionCreated += OnInteractionCreated;
        interaction.SlashCommandExecuted += OnSlashCommandExecuted;
        interaction.ContextCommandExecuted += OnContextCommandExecuted;
        interaction.ComponentCommandExecuted += OnComponentCommandExecuted;

        // Defer Discord client startup until the host (and Wolverine) is fully started
        applicationLifetime.ApplicationStarted.Register(OnApplicationStarted);

        logger.LogInformation("Hosted Service started.");
        return Task.CompletedTask;
    }

    private async void OnApplicationStarted()
    {
        try
        {
            await client.LoginAsync(TokenType.Bot, botOptions.Value.Token);
            await client.StartAsync();
            logger.LogInformation("Discord client connected.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occurred while starting the Discord client.");
        }
    }

    private async Task PublishNotification<TNotification>(TNotification notification)
    {
        if (_cts?.IsCancellationRequested == true)
        {
            return;
        }

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
            await bus.PublishAsync(notification);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish {NotificationType}", typeof(TNotification).Name);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Hosted Service stopping.");

        if (_cts is not null)
        {
            await _cts.CancelAsync();
        }

        client.Ready -= OnClientReady;
        client.Log -= OnClientLog;
        interaction.Log -= OnInteractionLog;
        client.MessageReceived -= OnMessageReceived;
        client.MessageUpdated -= OnMessageUpdated;
        client.MessageDeleted -= OnMessageDeleted;
        client.MessagesBulkDeleted -= OnMessagesBulkDeleted;
        client.ReactionAdded -= OnReactionAdded;
        client.ReactionRemoved -= OnReactionRemoved;
        client.ReactionsCleared -= OnReactionsCleared;
        client.ReactionsRemovedForEmote -= OnReactionsRemovedForEmote;
        client.ChannelCreated -= OnChannelCreated;
        client.ChannelUpdated -= OnChannelUpdated;
        client.ChannelDestroyed -= OnChannelDestroyed;
        client.GuildAvailable -= OnGuildAvailable;
        client.GuildUnavailable -= OnGuildUnavailable;
        client.GuildUpdated -= OnGuildUpdated;
        client.RoleCreated -= OnRoleCreated;
        client.RoleUpdated -= OnRoleUpdated;
        client.RoleDeleted -= OnRoleDeleted;
        client.UserUpdated -= OnUserUpdated;
        client.UserVoiceStateUpdated -= OnUserVoiceStateUpdated;
        client.UserIsTyping -= OnUserIsTyping;
        client.GuildMemberUpdated -= OnGuildMemberUpdated;
        client.GuildMembersDownloaded -= OnGuildMembersDownloaded;
        client.UserJoined -= OnUserJoined;
        client.UserLeft -= OnUserLeft;
        client.UserBanned -= OnUserBanned;
        client.UserUnbanned -= OnUserUnbanned;
        client.PresenceUpdated -= OnPresenceUpdated;
        client.GuildScheduledEventCreated -= OnGuildScheduledEventCreated;
        client.InviteCreated -= OnInviteCreated;
        client.InteractionCreated -= OnInteractionCreated;
        interaction.SlashCommandExecuted -= OnSlashCommandExecuted;
        interaction.ContextCommandExecuted -= OnContextCommandExecuted;
        interaction.ComponentCommandExecuted -= OnComponentCommandExecuted;

        await client.StopAsync();

        _cts?.Dispose();
    }

    #region Event Handlers

    private Task OnClientReady()
        => PublishNotification(new ClientReadyNotification());

    private Task OnClientLog(LogMessage message)
        => PublishNotification(new ClientLogNotification(message));

    private Task OnInteractionLog(LogMessage message)
        => PublishNotification(new InteractionLogNotification(message));

    private Task OnMessageReceived(SocketMessage message)
        => PublishNotification(new ClientMessageReceivedNotification(message));

    private Task OnMessageUpdated(Cacheable<Discord.IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        => PublishNotification(new ClientMessageUpdatedNotification(before, after, channel));

    private Task OnMessageDeleted(Cacheable<Discord.IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        => PublishNotification(new ClientMessageDeletedNotification(message, channel));

    private Task OnMessagesBulkDeleted(IReadOnlyCollection<Cacheable<Discord.IMessage, ulong>> messages, Cacheable<IMessageChannel, ulong> channel)
        => PublishNotification(new ClientMessagesBulkDeletedNotification(messages, channel));

    private Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        => PublishNotification(new ClientReactionAddedNotification(message, channel, reaction));

    private Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
        => PublishNotification(new ClientReactionRemovedNotification(message, channel, reaction));

    private Task OnReactionsCleared(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        => PublishNotification(new ClientReactionsClearedNotification(message, channel));

    private Task OnReactionsRemovedForEmote(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, IEmote emote)
        => PublishNotification(new ClientReactionsRemovedForEmoteNotification(message, channel, emote));

    private Task OnChannelCreated(SocketChannel channel)
        => PublishNotification(new ClientChannelCreatedNotification(channel));

    private Task OnChannelUpdated(SocketChannel before, SocketChannel after)
        => PublishNotification(new ClientChannelUpdatedNotification(before, after));

    private Task OnChannelDestroyed(SocketChannel channel)
        => PublishNotification(new ClientChannelDestroyedNotification(channel));

    private Task OnGuildAvailable(SocketGuild guild)
        => PublishNotification(new GuildAvailableNotification(guild));

    private Task OnGuildUnavailable(SocketGuild guild)
        => PublishNotification(new GuildUnavailableNotification(guild));

    private Task OnGuildUpdated(SocketGuild before, SocketGuild after)
        => PublishNotification(new GuildUpdatedNotification(before, after));

    private Task OnRoleCreated(SocketRole role)
        => PublishNotification(new GuildRoleCreatedNotification(role));

    private Task OnRoleUpdated(SocketRole before, SocketRole after)
        => PublishNotification(new GuildRoleUpdatedNotification(before, after));

    private Task OnRoleDeleted(SocketRole role)
        => PublishNotification(new GuildRoleDeletedNotification(role));

    private Task OnUserUpdated(SocketUser before, SocketUser after)
        => PublishNotification(new GuildUserUpdatedNotification(before, after));

    private Task OnUserVoiceStateUpdated(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        => PublishNotification(new GuildUserVoiceStateUpdatedNotification(user, before, after));

    private Task OnUserIsTyping(Cacheable<IUser, ulong> user, Cacheable<IMessageChannel, ulong> channel)
        => PublishNotification(new GuildUserIsTypingNotification(user, channel));

    private Task OnGuildMemberUpdated(Cacheable<SocketGuildUser, ulong> before, SocketGuildUser after)
        => PublishNotification(new GuildMemberUpdatedNotification(before, after));

    private Task OnGuildMembersDownloaded(SocketGuild guild)
        => PublishNotification(new GuildMembersDownloadedNotification(guild));

    private Task OnUserJoined(SocketGuildUser user)
        => PublishNotification(new GuildUserJoinedNotification(user));

    private Task OnUserLeft(SocketGuild guild, SocketUser user)
        => PublishNotification(new GuildUserLeftNotification(guild, user));

    private Task OnUserBanned(SocketUser user, SocketGuild guild)
        => PublishNotification(new GuildUserBannedNotification(user, guild));

    private Task OnUserUnbanned(SocketUser user, SocketGuild guild)
        => PublishNotification(new GuildUserUnbannedNotification(user, guild));

    private Task OnPresenceUpdated(SocketUser user, SocketPresence oldPresence, SocketPresence newPresence)
        => PublishNotification(new GuildPresenceUpdateNotification(user, oldPresence, newPresence));

    private Task OnGuildScheduledEventCreated(SocketGuildEvent guildEvent)
        => PublishNotification(new GuildScheduledEventNotification(guildEvent));

    private Task OnInviteCreated(SocketInvite invite)
        => PublishNotification(new GuildInviteCreatedNotification(invite));

    private Task OnInteractionCreated(SocketInteraction interaction)
        => PublishNotification(new ClientInteractionCreatedNotification(interaction));

    private Task OnSlashCommandExecuted(SlashCommandInfo info, IInteractionContext context, Discord.Interactions.IResult result)
        => PublishNotification(new InteractionSlashCommandExecutedNotification(info, context, result));

    private Task OnContextCommandExecuted(ContextCommandInfo info, IInteractionContext context, Discord.Interactions.IResult result)
        => PublishNotification(new InteractionContextCommandExecutedNotification(info, context, result));

    private Task OnComponentCommandExecuted(ComponentCommandInfo info, IInteractionContext context, Discord.Interactions.IResult result)
        => PublishNotification(new InteractionComponentCommandExecutedNotification(info, context, result));

    #endregion
}
