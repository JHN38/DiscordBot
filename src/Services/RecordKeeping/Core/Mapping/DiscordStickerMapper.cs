using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordSticker entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordStickerMapper
{
    /// <summary>
    /// Maps a DiscordStickerDto to a new DiscordSticker entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordStickerDto.GuildDiscordId))]
    [MapperIgnoreSource(nameof(DiscordStickerDto.CreatorDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordSticker.Id))]
    [MapperIgnoreTarget(nameof(DiscordSticker.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordSticker.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordSticker.GuildId))]
    [MapperIgnoreTarget(nameof(DiscordSticker.Guild))]
    [MapperIgnoreTarget(nameof(DiscordSticker.UserId))]
    [MapperIgnoreTarget(nameof(DiscordSticker.User))]
    [MapperIgnoreTarget(nameof(DiscordSticker.Messages))]
    public static partial DiscordSticker ToEntity(this DiscordStickerDto dto);
}
