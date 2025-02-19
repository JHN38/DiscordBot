using Discord;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Events;

public record GuildSystemMessageReceivedNotification(ISystemMessage Message) : INotification;

public class GuildSystemMessageReceivedHandler(ILogger<GuildSystemMessageReceivedHandler> logger) : INotificationHandler<GuildSystemMessageReceivedNotification>
{
    public async Task Handle(GuildSystemMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var msg = message.Content;
        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;

        switch (message.Type)
        {
            case MessageType.ChannelPinnedMessage:
                logger.LogInformation("({Guild}) #{Channel} [System Message: Pinned Message] {Message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.RecipientAdd:
                logger.LogInformation("({Guild}) #{Channel} [System Message: Recipient Added] {Message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.RecipientRemove:
                logger.LogInformation("({Guild}) #{Channel} [System Message: Recipient Removed] {Message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.Call:
                logger.LogInformation("({Guild}) #{Channel} [System Message: Call] {Message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.ChannelNameChange:
                logger.LogInformation("({Guild}) #{Channel} [System Message: Channel Name Changed] {Message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.ChannelIconChange:
                logger.LogInformation("({Guild}) #{Channel} [System Message: Channel Icon Changed] {Message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.GuildMemberJoin:
                logger.LogInformation("({Guild}) #{Channel} [System Message: Guild Member Joined] {Message}", guild.Name, guildChannel.Name, msg);
                break;

            default:
                logger.LogInformation("({Guild}) #{Channel} [System Message] {Message}", guild.Name, guildChannel.Name, msg);
                break;
        }

        await Task.CompletedTask;
    }
}
