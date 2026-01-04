using Discord;
using DiscordBot.Bot.Core.Domain;
using DiscordBot.Bot.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DiscordBot.Bot.Core.Services;

public sealed class DiscordUserDisplayNameResolver(IDiscordClient client, IMemoryCache cache) : IDiscordUserDisplayNameResolver
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public async Task<Dictionary<GuildUserDiscordIdPair, string>> ResolveDisplayNamesAsync(
        IEnumerable<GuildUserDiscordIdPair> guildUserPairs,
        ulong fallbackGuildDiscordId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (guildUserPairs?.Any() != true) return [];

        var resolvedDisplayNames = new Dictionary<GuildUserDiscordIdPair, string>();
        var pairsToResolve = new List<GuildUserDiscordIdPair>();

        // Check cache first
        foreach (var pair in guildUserPairs)
        {
            var cacheKey = GetCacheKey(pair);
            if (cache.TryGetValue(cacheKey, out string? cachedName) && cachedName is not null)
            {
                resolvedDisplayNames[pair] = cachedName;
            }
            else
            {
                pairsToResolve.Add(pair);
            }
        }

        if (pairsToResolve.Count == 0)
            return resolvedDisplayNames;

        // Group by guild to batch API calls
        var pairsByGuild = pairsToResolve.GroupBy(p => p.GuildDiscordId);

        foreach (var guildGroup in pairsByGuild)
        {
            var guildId = guildGroup.Key;

            if (await client.GetGuildAsync(guildId, options: new() { CancelToken = cancellationToken }) is { } guild)
            {
                // Download all users for this guild at once if needed
                if (guild is IGuild socketGuild)
                {
                    await socketGuild.DownloadUsersAsync();
                }

                foreach (var pair in guildGroup)
                {
                    if (await guild.GetUserAsync(pair.UserDiscordId, options: new() { CancelToken = cancellationToken }) is { } guildUser)
                    {
                        var displayName = guildUser.DisplayName ?? guildUser.GlobalName ?? guildUser.Username;
                        CacheAndStore(pair, displayName, resolvedDisplayNames);
                        continue;
                    }

                    // Try fallback guild
                    if (guildId != fallbackGuildDiscordId)
                    {
                        await TryResolveFallbackAsync(pair, fallbackGuildDiscordId, resolvedDisplayNames, cancellationToken);
                    }
                }
            }
            else
            {
                // Guild not accessible, try fallback for all pairs in this group
                foreach (var pair in guildGroup)
                {
                    await TryResolveFallbackAsync(pair, fallbackGuildDiscordId, resolvedDisplayNames, cancellationToken);
                }
            }
        }

        // For any remaining unresolved pairs, try global username
        foreach (var pair in pairsToResolve.Where(p => !resolvedDisplayNames.ContainsKey(p)))
        {
            await ResolveGlobalUsernameAsync(pair, resolvedDisplayNames, cancellationToken);
        }

        return resolvedDisplayNames;
    }

    private async Task TryResolveFallbackAsync(
        GuildUserDiscordIdPair pair,
        ulong fallbackGuildDiscordId,
        Dictionary<GuildUserDiscordIdPair, string> resolvedDisplayNames,
        CancellationToken cancellationToken)
    {
        if (await client.GetGuildAsync(fallbackGuildDiscordId, options: new() { CancelToken = cancellationToken }) is not { } fallbackGuild)
            return;

        if (await fallbackGuild.GetUserAsync(pair.UserDiscordId, options: new() { CancelToken = cancellationToken }) is not { } guildUser)
            return;

        var displayName = guildUser.DisplayName ?? guildUser.GlobalName ?? guildUser.Username;
        CacheAndStore(pair, displayName, resolvedDisplayNames);
    }

    private async Task ResolveGlobalUsernameAsync(
        GuildUserDiscordIdPair pair,
        Dictionary<GuildUserDiscordIdPair, string> resolvedDisplayNames,
        CancellationToken cancellationToken)
    {
        if (await client.GetUserAsync(pair.UserDiscordId, options: new() { CancelToken = cancellationToken }) is not { } user)
            return;

        var displayName = user.GlobalName ?? user.Username;
        CacheAndStore(pair, displayName, resolvedDisplayNames);
    }

    private void CacheAndStore(GuildUserDiscordIdPair pair, string displayName, Dictionary<GuildUserDiscordIdPair, string> resolvedDisplayNames)
    {
        resolvedDisplayNames[pair] = displayName;
        cache.Set(GetCacheKey(pair), displayName, CacheDuration);
    }

    private static string GetCacheKey(GuildUserDiscordIdPair pair) => $"DisplayName:{pair.GuildDiscordId}:{pair.UserDiscordId}";
}
