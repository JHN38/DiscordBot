using DiscordBot.Bot.Core.Domain;

namespace DiscordBot.Bot.Core.Interfaces;

public interface IDiscordUserDisplayNameResolver
{
    Task<Dictionary<GuildUserDiscordIdPair, string>> ResolveDisplayNamesAsync(IEnumerable<GuildUserDiscordIdPair> guildUserPairs, ulong fallbackGuildDiscordId, CancellationToken cancellationToken = default);
}
