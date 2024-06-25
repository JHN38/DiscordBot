using System.Runtime.Intrinsics.X86;
using ChatGptNet;
using ChatGptNet.Extensions;
using DiscordBot.Domain.ChatGpt.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.ChatGpt.Commands;

public class ChatGptRequestHandler(ILogger<ChatGptRequestHandler> logger, IChatGptClient chatGptClient) : IRequestHandler<ChatGptRequest, string?>
{
    public async Task<string?> Handle(ChatGptRequest request, CancellationToken cancellationToken)
    {
        // You will be replying into a ```md{Your answer here}``` code block on Discord, therefor you need to format your answer using markdowns.
        // Do not use any form of Discord formatting like asterisks, underscores, etc... Only markdown will work.

        var conversationId = await chatGptClient.SetupAsync("""
            Keep your answers brief and to the point. 
            You will be replying into a Discord Block Quotes using ">>> ". Do not add a case return after ">>> " or the block will not work.
            Use a lot of discord text formatting to improve user readability. To work correctly, lists should not be put in any form of markdown such as asterisks, underscores, italic, etc.
            DO NOT put 1. 2. 3. etc inside bold or italic markdowns.
            """, cancellationToken: cancellationToken);
        var response = await chatGptClient.AskAsync(conversationId, request.Query, cancellationToken: cancellationToken);

        if (!response.IsSuccessful)
        {
            logger.LogWarning("Failed to generate response for mention by user {User} for prompt \"{Prompt}\".\r\nIsContentFiltered: {IsContentFiltered} / IsPromptFiltered: {IsPromptFiltered}",
                request.User, request.Query, response.IsContentFiltered, response.IsPromptFiltered);
            return null;
        }

        return response.GetContent();
    }
}
