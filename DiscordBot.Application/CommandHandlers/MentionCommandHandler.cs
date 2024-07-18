using Discord;
using DiscordBot.Application.Common.Helpers;
using DiscordBot.Domain.Commands;
using DiscordBot.Domain.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands;

public class MentionCommandHandler(ILogger<MentionCommandHandler> logger,
                                               IMediator mediator) : IRequestHandler<MentionCommand, IUserMessage?>
{
    public async Task<IUserMessage?> Handle(MentionCommand notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        if (MessageContentHelper.StripBotMention(message.Content) is not string messageContent || string.IsNullOrWhiteSpace(messageContent))
            return null;

        using var typingState = message.Channel.EnterTypingState();

        try
        {
            return await mediator.Send(new ChatGptEmbedRequest(message, messageContent), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the ChatGPT request.");
            return await message.ReplyAsync("Sorry, I couldn't process your request at the moment.");
        }
    }
}
