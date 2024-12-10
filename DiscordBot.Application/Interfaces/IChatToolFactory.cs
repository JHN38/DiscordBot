using OpenAI.Chat;

namespace DiscordBot.Application.Interfaces
{
    public interface IChatToolFactory
    {
        IEnumerable<ChatTool> GetTools();
        Task<string> ExecuteToolAsync(ChatToolCall toolCall, CancellationToken cancellationToken = default);
    }
}
