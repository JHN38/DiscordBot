namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord invite information.
/// </summary>
public sealed record DiscordInviteDto(
    ulong DiscordId,
    string Code,
    ulong GuildId,
    ulong ChannelId,
    ulong? InviterDiscordId = null,
    int? MaxAge = null,
    int? MaxUses = null,
    int Uses = 0,
    bool IsTemporary = false,
    DateTimeOffset? ExpiresAt = null,
    int? TargetType = null,
    ulong? TargetUserId = null,
    ulong? TargetApplicationId = null);
