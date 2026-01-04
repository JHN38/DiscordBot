using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordVoiceSession entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordVoiceSessionMapper
{
    /// <summary>
    /// Maps a DiscordVoiceSessionDto to a new DiscordVoiceSession entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordVoiceSessionDto.UserDiscordId))]
    [MapperIgnoreSource(nameof(DiscordVoiceSessionDto.ChannelDiscordId))]
    [MapperIgnoreSource(nameof(DiscordVoiceSessionDto.GuildDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordVoiceSession.Id))]
    [MapperIgnoreTarget(nameof(DiscordVoiceSession.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordVoiceSession.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordVoiceSession.UserId))]
    [MapperIgnoreTarget(nameof(DiscordVoiceSession.User))]
    [MapperIgnoreTarget(nameof(DiscordVoiceSession.ChannelId))]
    [MapperIgnoreTarget(nameof(DiscordVoiceSession.Channel))]
    [MapperIgnoreTarget(nameof(DiscordVoiceSession.GuildId))]
    [MapperIgnoreTarget(nameof(DiscordVoiceSession.Guild))]
    public static partial DiscordVoiceSession ToEntity(this DiscordVoiceSessionDto dto);
}
