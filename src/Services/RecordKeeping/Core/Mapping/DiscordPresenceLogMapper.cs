using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordPresenceLog entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordPresenceLogMapper
{
    /// <summary>
    /// Maps a DiscordPresenceLogDto to a new DiscordPresenceLog entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordPresenceLogDto.UserDiscordId))]
    [MapperIgnoreSource(nameof(DiscordPresenceLogDto.GuildDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordPresenceLog.Id))]
    [MapperIgnoreTarget(nameof(DiscordPresenceLog.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordPresenceLog.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordPresenceLog.UserId))]
    [MapperIgnoreTarget(nameof(DiscordPresenceLog.User))]
    [MapperIgnoreTarget(nameof(DiscordPresenceLog.GuildId))]
    [MapperIgnoreTarget(nameof(DiscordPresenceLog.Guild))]
    public static partial DiscordPresenceLog ToEntity(this DiscordPresenceLogDto dto);
}
