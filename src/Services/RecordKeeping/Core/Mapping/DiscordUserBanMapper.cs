using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordUserBan entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordUserBanMapper
{
    /// <summary>
    /// Maps a DiscordUserBanDto to a new DiscordUserBan entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordUserBanDto.UserDiscordId))]
    [MapperIgnoreSource(nameof(DiscordUserBanDto.GuildDiscordId))]
    [MapperIgnoreSource(nameof(DiscordUserBanDto.BannedByDiscordId))]
    [MapperIgnoreSource(nameof(DiscordUserBanDto.UnbannedByDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.Id))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.UserId))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.User))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.GuildId))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.Guild))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.BannedById))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.BannedBy))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.UnbannedById))]
    [MapperIgnoreTarget(nameof(DiscordUserBan.UnbannedBy))]
    public static partial DiscordUserBan ToEntity(this DiscordUserBanDto dto);
}
