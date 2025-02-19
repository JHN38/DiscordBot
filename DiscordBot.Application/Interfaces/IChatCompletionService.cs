using OpenAI.Chat;

namespace DiscordBot.Application.Interfaces;

public interface IChatCompletionService
{
    Task<ChatCompletion> GenerateChatCompletionAsync(List<ChatMessage> messages, ChatCompletionOptions options, CancellationToken cancellationToken = default);
}
