using DiscordBot.Application.Services;
using DiscordBot.Domain.Entities;

namespace DiscordBot.Application.Interfaces;

public interface IDiscordUserDisplayNameResolver
{
    Task<Dictionary<DiscordGuildUserPair, string>> ResolveDisplayNamesAsync(IEnumerable<DiscordGuildUserPair> guildUserPairs, ulong fallbackGuildId, CancellationToken cancellationToken = default);
}
