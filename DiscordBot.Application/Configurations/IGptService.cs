using Discord;
using DiscordBot.Domain.Entities;

namespace DiscordBot.Application.Configurations;

/// <summary>
/// Interface for GPT service that provides methods for interacting with Discord users.
/// </summary>
public interface IGptService
{
    Task<string> DiscordRenameUserAsync(ChatGptDiscordRenameUserArguments arguments, IMessage message, CancellationToken cancellationToken = default);

    Task<string> DiscordTimeoutUserAsync(ChatGptDiscordTimeoutUserArguments arguments, CancellationToken cancellationToken = default);
}
