using Discord;
using DiscordBot.Domain.DirectMessage.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Discord.DirectMessage.Events;

public class DirectSystemMessageReceivedHandler(ILogger<DirectSystemMessageReceivedHandler> logger) : INotificationHandler<DirectSystemMessageReceivedNotification>
{
    public async Task Handle(DirectSystemMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var msg = message.Content;

        switch (message.Type)
        {
            case MessageType.ChannelPinnedMessage:
                logger.LogInformation("[Direct Message: Pinned Message] {message}", msg);
                break;

            case MessageType.RecipientAdd:
                logger.LogInformation("[Direct Message: Recipient Added] {message}", msg);
                break;

            case MessageType.RecipientRemove:
                logger.LogInformation("[Direct Message: Recipient Removed] {message}", msg);
                break;

            case MessageType.Call:
                logger.LogInformation("[Direct Message: Call] {message}", msg);
                break;

            case MessageType.ChannelNameChange:
                logger.LogInformation("[Direct Message: Channel Name Changed] {message}", msg);
                break;

            case MessageType.ChannelIconChange:
                logger.LogInformation("[Direct Message: Channel Icon Changed] {message}", msg);
                break;

            case MessageType.GuildMemberJoin:
                logger.LogInformation("[Direct Message: Guild Member Joined] {message}", msg);
                break;

            // Add cases for other specific system message types as needed

            default:
                logger.LogInformation("[Direct Message: System Message] {message}", msg);
                break;
        }

        await Task.CompletedTask;
    }
}
