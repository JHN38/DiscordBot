using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordAttachment entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordAttachmentMapper
{
    /// <summary>
    /// Maps a DiscordAttachmentDto to a new DiscordAttachment entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordAttachmentDto.MessageDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordAttachment.Id))]
    [MapperIgnoreTarget(nameof(DiscordAttachment.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordAttachment.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordAttachment.MessageId))]
    [MapperIgnoreTarget(nameof(DiscordAttachment.Message))]
    public static partial DiscordAttachment ToEntity(this DiscordAttachmentDto dto);
}
