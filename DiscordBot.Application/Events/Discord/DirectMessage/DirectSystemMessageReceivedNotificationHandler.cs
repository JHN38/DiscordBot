using Discord;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Events;

public record DirectSystemMessageReceivedNotification(ISystemMessage Message) : INotification;

public class DirectSystemMessageReceivedHandler(ILogger<DirectSystemMessageReceivedHandler> logger) : INotificationHandler<DirectSystemMessageReceivedNotification>
{
    public async Task Handle(DirectSystemMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var msg = message.Content;

        switch (message.Type)
        {
            case MessageType.ChannelPinnedMessage:
                logger.LogInformation("[Direct Message: Pinned Message] {Message}", msg);
                break;

            case MessageType.RecipientAdd:
                logger.LogInformation("[Direct Message: Recipient Added] {Message}", msg);
                break;

            case MessageType.RecipientRemove:
                logger.LogInformation("[Direct Message: Recipient Removed] {Message}", msg);
                break;

            case MessageType.Call:
                logger.LogInformation("[Direct Message: Call] {Message}", msg);
                break;

            case MessageType.ChannelNameChange:
                logger.LogInformation("[Direct Message: Channel Name Changed] {Message}", msg);
                break;

            case MessageType.ChannelIconChange:
                logger.LogInformation("[Direct Message: Channel Icon Changed] {Message}", msg);
                break;

            case MessageType.GuildMemberJoin:
                logger.LogInformation("[Direct Message: Guild Member Joined] {Message}", msg);
                break;

            // Add cases for other specific system message types as needed

            default:
                logger.LogInformation("[Direct Message: System Message] {Message}", msg);
                break;
        }

        await Task.CompletedTask;
    }
}
