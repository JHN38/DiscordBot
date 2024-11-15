using Discord;
using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Application.Events;
using DiscordBot.Domain.Common;
using DiscordBot.Domain.Common.Extensions;
using DiscordBot.Domain.Entities;
using MediatR;

namespace DiscordBot.Application.Messages.Events;

public class CreateMessageEventHandler(IRepository<DiscordGuild> guildRepository,
                                       IRepository<DiscordChannel> channelRepository,
                                       IRepository<DiscordUser> userRepository,
                                       IRepository<DiscordMessage> messageRepository)
    : INotificationHandler<GuildUserMessageReceivedNotification>
{
    public async Task Handle(GuildUserMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        if (message is not { Channel: IGuildChannel guildChannel })
        {
            // Handle messages not from a guild channel
            return;
        }

        var guild = guildChannel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id, CacheMode.AllowDownload, new() { CancelToken = cancellationToken });

        if (author is null)
        {
            // Handle author not found
            return;
        }

        var discordGuild = await GetOrCreateEntityAsync(
            guildRepository,
            guild.Id,
            guild.ToDiscordGuild,
            cancellationToken);

        var discordUser = await GetOrCreateEntityAsync(
            userRepository,
            author.Id,
            author.ToDiscordUser,
            cancellationToken);

        var discordChannel = await GetOrCreateEntityAsync(
            channelRepository,
            guildChannel.Id,
            guildChannel.ToDiscordChannel,
            cancellationToken);

        var discordMessage = message.ToDiscordMessage(discordChannel, discordUser);

        await SaveEntitiesAsync(discordGuild, discordUser, discordChannel, discordMessage, cancellationToken);
    }

    private async Task SaveEntitiesAsync(
        DiscordGuild discordGuild,
        DiscordUser discordUser,
        DiscordChannel discordChannel,
        DiscordMessage discordMessage,
        CancellationToken cancellationToken)
    {
        // TODO: Move this to a Repository class so that transactions can be used
        // Start a transaction
        //using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Save entities in order respecting foreign keys
            //await AddIfNewAsync(guildRepository, discordGuild, cancellationToken);
            //await AddIfNewAsync(userRepository, discordUser, cancellationToken);
            //await AddIfNewAsync(channelRepository, discordChannel, cancellationToken);
            await messageRepository.AddAsync(discordMessage, cancellationToken);

            // Commit transaction
            //await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            // Rollback transaction on error
            //await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task<TEntity> GetOrCreateEntityAsync<TEntity>(IRepository<TEntity> repository,
                                                                       ulong id,
                                                                       Func<TEntity> createEntity,
                                                                       CancellationToken cancellationToken)
        where TEntity : DiscordEntity =>
    await repository.GetByIdAsync(id, cancellationToken) ?? createEntity();

    private static Task AddIfNewAsync<TEntity>(
        IRepository<TEntity> repository,
        TEntity entity,
        CancellationToken cancellationToken)
        where TEntity : DiscordEntity =>
        entity.Id == 0 ? repository.AddAsync(entity, cancellationToken) : Task.CompletedTask;

}
