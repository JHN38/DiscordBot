using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordUserNickname entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordUserNicknameMapper
{
    /// <summary>
    /// Maps a DiscordUserNicknameDto to a new DiscordUserNickname entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordUserNicknameDto.UserDiscordId))]
    [MapperIgnoreSource(nameof(DiscordUserNicknameDto.GuildDiscordId))]
    [MapperIgnoreSource(nameof(DiscordUserNicknameDto.ChangedByDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordUserNickname.Id))]
    [MapperIgnoreTarget(nameof(DiscordUserNickname.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordUserNickname.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordUserNickname.IsActive))]
    [MapperIgnoreTarget(nameof(DiscordUserNickname.UserId))]
    [MapperIgnoreTarget(nameof(DiscordUserNickname.User))]
    [MapperIgnoreTarget(nameof(DiscordUserNickname.GuildId))]
    [MapperIgnoreTarget(nameof(DiscordUserNickname.Guild))]
    [MapperIgnoreTarget(nameof(DiscordUserNickname.ChangedById))]
    [MapperIgnoreTarget(nameof(DiscordUserNickname.ChangedBy))]
    public static partial DiscordUserNickname ToEntity(this DiscordUserNicknameDto dto);
}
