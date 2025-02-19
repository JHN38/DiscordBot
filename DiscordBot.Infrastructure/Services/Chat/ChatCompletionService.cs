using DiscordBot.Application.Interfaces;
using OpenAI;
using OpenAI.Chat;

namespace DiscordBot.Infrastructure.Services
{
    public class ChatCompletionService(OpenAIClient openAIClient) : IChatCompletionService
    {
        private readonly OpenAIClient _openAIClient = openAIClient;

        public async Task<ChatCompletion> GenerateChatCompletionAsync(List<ChatMessage> messages, ChatCompletionOptions options, CancellationToken cancellationToken = default)
        {
            // Use the OpenAI SDK to get the chat completion
            var chatClient = _openAIClient.GetChatClient("gpt-4o");
            return await chatClient.CompleteChatAsync(messages, options, cancellationToken);
        }
    }
}
