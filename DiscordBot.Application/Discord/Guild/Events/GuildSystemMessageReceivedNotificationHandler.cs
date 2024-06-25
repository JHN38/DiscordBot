using Discord;
using DiscordBot.Domain.Guild.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Discord.Guild.Events;

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
                logger.LogInformation("({guild}) #{channel} [System Message: Pinned Message] {message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.RecipientAdd:
                logger.LogInformation("({guild}) #{channel} [System Message: Recipient Added] {message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.RecipientRemove:
                logger.LogInformation("({guild}) #{channel} [System Message: Recipient Removed] {message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.Call:
                logger.LogInformation("({guild}) #{channel} [System Message: Call] {message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.ChannelNameChange:
                logger.LogInformation("({guild}) #{channel} [System Message: Channel Name Changed] {message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.ChannelIconChange:
                logger.LogInformation("({guild}) #{channel} [System Message: Channel Icon Changed] {message}", guild.Name, guildChannel.Name, msg);
                break;

            case MessageType.GuildMemberJoin:
                logger.LogInformation("({guild}) #{channel} [System Message: Guild Member Joined] {message}", guild.Name, guildChannel.Name, msg);
                break;

            default:
                logger.LogInformation("({guild}) #{channel} [System Message] {message}", guild.Name, guildChannel.Name, msg);
                break;
        }

        await Task.CompletedTask;
    }
}
