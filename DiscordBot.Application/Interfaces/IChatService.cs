using Discord;

namespace DiscordBot.Application.Interfaces
{
    public interface IChatService
    {
        Task<string> ProcessMessageAsync(IUserMessage discordMessage, string message, CancellationToken cancellationToken = default);
    }
}
