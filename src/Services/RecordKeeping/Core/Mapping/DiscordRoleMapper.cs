using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordRole entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordRoleMapper
{
    /// <summary>
    /// Maps a DiscordRoleDto to a new DiscordRole entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreTarget(nameof(DiscordRole.Id))]
    [MapperIgnoreTarget(nameof(DiscordRole.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordRole.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordRole.IsDeleted))]
    [MapperIgnoreTarget(nameof(DiscordRole.DeletedAt))]
    [MapperIgnoreTarget(nameof(DiscordRole.GuildId))]
    [MapperIgnoreTarget(nameof(DiscordRole.Guild))]
    [MapperIgnoreTarget(nameof(DiscordRole.Users))]
    public static partial DiscordRole ToEntity(this DiscordRoleDto dto);
}
