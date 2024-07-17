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

        using (message.Channel.EnterTypingState()) try
            {
                await mediator.Send(new ChatGptEmbedRequest(message, messageContent), cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the ChatGPT request.");
                await message.ReplyAsync("Sorry, I couldn't process your request at the moment.");
            }
    }
}
