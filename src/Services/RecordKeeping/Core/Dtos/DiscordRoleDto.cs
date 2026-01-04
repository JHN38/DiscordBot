using DiscordBot.Service.RecordKeeping.Core.Domain.Enums;

namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Data transfer object for Discord role information.
/// </summary>
public sealed record DiscordRoleDto(
    ulong DiscordId,
    string Name,
    int Color,
    bool IsHoisted,
    int Position,
    DiscordPermissions Permissions,
    bool IsManaged,
    bool IsMentionable,
    string? IconUrl = null,
    string? UnicodeEmoji = null,
    string? Tags = null);
