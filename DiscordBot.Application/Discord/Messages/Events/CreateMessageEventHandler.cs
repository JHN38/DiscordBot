using Discord;
using DiscordBot.Application.Events;
using MediatR;

namespace DiscordBot.Application.Discord.Messages.Events;

public class CreateMessageEventHandler(IDiscordMessageRepository messageRepository) : INotificationHandler<GuildUserMessageReceivedNotification>
{
    public async Task Handle(GuildUserMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        if (message is not { Channel: IGuildChannel channel })
        {
            // Handle messages not from a guild channel
            return;
        }

        var guild = channel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id, CacheMode.AllowDownload, new() { CancelToken = cancellationToken });

        if (author is null)
        {
            // Handle author not found
            return;
        }

        if (message.ReferencedMessage is { } referencedMessage)
        {
            if (referencedMessage is not { Channel: IGuildChannel referencedMessageChannel })
            {
                // Handle messages not from a guild channel
                return;
            }

            // Add the referenced message (if any) first
            await messageRepository.GetOrCreateEntityAsync(message.ReferencedMessage.Id, () =>
                messageRepository.AddWithDependenciesAsync(referencedMessage, referencedMessageChannel,
                    referencedMessageChannel.Guild, referencedMessage.Author, cancellationToken),
                cancellationToken);
        }

        await messageRepository.AddWithDependenciesAsync(message, channel, guild, author, cancellationToken);
    }
}
