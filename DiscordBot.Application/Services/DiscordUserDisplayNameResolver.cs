using Discord;
using DiscordBot.Application.Interfaces;
using DiscordBot.Domain.Entities;

namespace DiscordBot.Application.Services;

public class DiscordUserDisplayNameResolver(IDiscordClient client) : IDiscordUserDisplayNameResolver
// Primary constructor to initialize the client
{
    private readonly IDiscordClient _client = client ?? throw new ArgumentNullException(nameof(client));

    /// <summary>
    /// Resolves display names for users involved in a collection of Discord messages.
    /// Attempts to resolve based on the guild of the original message, then a fallback guild, and finally by username.
    /// </summary>
    public async Task<Dictionary<DiscordGuildUserPair, string>> ResolveDisplayNamesAsync(IEnumerable<DiscordGuildUserPair> guildUserPairs, ulong fallbackGuildId, CancellationToken cancellationToken = default)
    {
        if (!guildUserPairs?.Any() ?? true) return []; // Early exit for null or empty input

        // Use a collection expression to build the resolved names dictionary
        var resolvedDisplayNames = new Dictionary<DiscordGuildUserPair, string>();

        foreach (var guildUserPair in guildUserPairs!)
        {
            // Try to resolve the display name using cascading resolution steps
            _ = await ResolveFromGuildAsync(guildUserPair, resolvedDisplayNames, cancellationToken) ||
                await ResolveFromFallbackGuildAsync(guildUserPair, fallbackGuildId, resolvedDisplayNames, cancellationToken) ||
                await ResolveGlobalUsernameAsync(guildUserPair, resolvedDisplayNames, cancellationToken);
        }

        return resolvedDisplayNames;
    }

    /// <summary>
    /// Attempts to resolve a display name for a user from their guild context.
    /// </summary>
    private async Task<bool> ResolveFromGuildAsync(DiscordGuildUserPair pair, Dictionary<DiscordGuildUserPair, string> resolvedDisplayNames, CancellationToken cancellationToken = default)
    {
        if (await _client.GetGuildAsync(pair.GuildId, options: new() { CancelToken = cancellationToken }) is not { } guild) return false;
        if (await guild.GetUserAsync(pair.UserId, options: new() { CancelToken = cancellationToken }) is not { } guildUser) return false;

        resolvedDisplayNames[pair] = guildUser.DisplayName ?? guildUser.GlobalName ?? guildUser.Username;
        return true;
    }

    /// <summary>
    /// Attempts to resolve a display name from the fallback guild for the given user ID.
    /// </summary>
    private Task<bool> ResolveFromFallbackGuildAsync(DiscordGuildUserPair pair, ulong fallbackGuildId, Dictionary<DiscordGuildUserPair, string> resolvedDisplayNames, CancellationToken cancellationToken = default) =>
        ResolveFromGuildAsync(new DiscordGuildUserPair(fallbackGuildId, pair.UserId), resolvedDisplayNames, cancellationToken);

    /// <summary>
    /// Attempts to resolve a user's global username.
    /// </summary>
    private async Task<bool> ResolveGlobalUsernameAsync(DiscordGuildUserPair pair, Dictionary<DiscordGuildUserPair, string> resolvedDisplayNames, CancellationToken cancellationToken = default)
    {
        if (await _client.GetUserAsync(pair.UserId, options: new() { CancelToken = cancellationToken }) is not { } user) return false;

        resolvedDisplayNames[pair] = user.GlobalName ?? user.Username;
        return true;
    }
}
