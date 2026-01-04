using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordEmbed entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordEmbedMapper
{
    /// <summary>
    /// Maps a DiscordEmbedDto to a new DiscordEmbed entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordEmbedDto.MessageDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordEmbed.Id))]
    [MapperIgnoreTarget(nameof(DiscordEmbed.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordEmbed.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordEmbed.MessageId))]
    [MapperIgnoreTarget(nameof(DiscordEmbed.Message))]
    public static partial DiscordEmbed ToEntity(this DiscordEmbedDto dto);
}
