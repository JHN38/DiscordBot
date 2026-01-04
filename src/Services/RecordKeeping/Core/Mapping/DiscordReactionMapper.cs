using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordReaction entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordReactionMapper
{
    /// <summary>
    /// Maps a DiscordReactionDto to a new DiscordReaction entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordReactionDto.MessageDiscordId))]
    [MapperIgnoreSource(nameof(DiscordReactionDto.UserDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordReaction.Id))]
    [MapperIgnoreTarget(nameof(DiscordReaction.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordReaction.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordReaction.RemovedAt))]
    [MapperIgnoreTarget(nameof(DiscordReaction.MessageId))]
    [MapperIgnoreTarget(nameof(DiscordReaction.Message))]
    [MapperIgnoreTarget(nameof(DiscordReaction.UserId))]
    [MapperIgnoreTarget(nameof(DiscordReaction.User))]
    public static partial DiscordReaction ToEntity(this DiscordReactionDto dto);
}
