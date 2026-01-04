using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordScheduledEvent entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordScheduledEventMapper
{
    /// <summary>
    /// Maps a DiscordScheduledEventDto to a new DiscordScheduledEvent entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordScheduledEventDto.GuildDiscordId))]
    [MapperIgnoreSource(nameof(DiscordScheduledEventDto.ChannelDiscordId))]
    [MapperIgnoreSource(nameof(DiscordScheduledEventDto.CreatorDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordScheduledEvent.Id))]
    [MapperIgnoreTarget(nameof(DiscordScheduledEvent.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordScheduledEvent.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordScheduledEvent.GuildId))]
    [MapperIgnoreTarget(nameof(DiscordScheduledEvent.Guild))]
    [MapperIgnoreTarget(nameof(DiscordScheduledEvent.ChannelId))]
    [MapperIgnoreTarget(nameof(DiscordScheduledEvent.Channel))]
    [MapperIgnoreTarget(nameof(DiscordScheduledEvent.CreatorId))]
    [MapperIgnoreTarget(nameof(DiscordScheduledEvent.Creator))]
    public static partial DiscordScheduledEvent ToEntity(this DiscordScheduledEventDto dto);
}
