using Discord;
using DiscordBot.Application.Common.Helpers;
using DiscordBot.Domain.ChatGpt.Commands;
using DiscordBot.Domain.Commands.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands.Events;

public class MentionCommandNotificationHandler(ILogger<MentionCommandNotificationHandler> logger,
                                               IMediator mediator) : INotificationHandler<MentionCommandNotification>
{
    public async Task Handle(MentionCommandNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        if (MessageContentHelper.StripBotMention(message.Content) is not string messageContent)
            return;

        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id);
        var user = author.DisplayName ?? author.GlobalName ?? author.Username;

        using (message.Channel.EnterTypingState()) try
            {
                var response = await mediator.Send(new ChatGptRequest(user, messageContent), cancellationToken);

                if (string.IsNullOrWhiteSpace(response))
                {
                    logger.LogWarning("Prompt {Prompt} by user {User} yielded no response.", messageContent, user);
                    return;
                }

                logger.LogDebug("Responding to mention by user {User} with: {Response}", user, response);

                foreach (var chunk in MessageContentHelper.SplitResponseIntoChunks(response, 1950))
                {
                    await message.ReplyAsync(chunk);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the ChatGPT request.");
                await message.ReplyAsync("Sorry, I couldn't process your request at the moment.");
            }
    }
}
