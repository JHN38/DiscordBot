using Discord;
using DiscordBot.Application.Commands;
using DiscordBot.Application.Configurations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Events;

public record GuildUserMessageReceivedNotification(IUserMessage Message) : INotification;

public sealed partial class GuildUserMessageReceivedHandler(ILogger<GuildUserMessageReceivedHandler> logger,
                                                            IMediator mediator,
                                                            IBotConfig config) : INotificationHandler<GuildUserMessageReceivedNotification>
{
    public async Task Handle(GuildUserMessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;

        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;

        var author = await guild.GetUserAsync(message.Author.Id);

        logger.LogInformation("({Guild}) #{Channel} <{User}> {Message}",
            guild.Name, guildChannel.Name, author.DisplayName, message.CleanContent);

        if (config.TextCommandPrefix is string prefix && message.Content.StartsWith(prefix))
        {
            await mediator.Send(new TextCommand(message, message.Content[prefix.Length..]), cancellationToken);
            return;
        }

        // @bot mention command
        var botUser = await guildChannel.Guild.GetCurrentUserAsync();
        if (BotMentionedFirst(message, botUser))
        {
            await mediator.Send(new MentionCommand(message), cancellationToken);
        }
    }

    public static bool BotMentionedFirst(IUserMessage message, IGuildUser botUser)
        => message.Content.StartsWith(botUser.Mention);
}
