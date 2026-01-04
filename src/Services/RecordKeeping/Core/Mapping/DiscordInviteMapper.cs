using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordInvite entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordInviteMapper
{
    /// <summary>
    /// Maps a DiscordInviteDto to a new DiscordInvite entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordInviteDto.GuildId))]
    [MapperIgnoreSource(nameof(DiscordInviteDto.ChannelId))]
    [MapperIgnoreSource(nameof(DiscordInviteDto.InviterDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordInvite.Id))]
    [MapperIgnoreTarget(nameof(DiscordInvite.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordInvite.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordInvite.IsRevoked))]
    [MapperIgnoreTarget(nameof(DiscordInvite.RevokedAt))]
    [MapperIgnoreTarget(nameof(DiscordInvite.GuildId))]
    [MapperIgnoreTarget(nameof(DiscordInvite.Guild))]
    [MapperIgnoreTarget(nameof(DiscordInvite.ChannelId))]
    [MapperIgnoreTarget(nameof(DiscordInvite.Channel))]
    [MapperIgnoreTarget(nameof(DiscordInvite.InviterId))]
    [MapperIgnoreTarget(nameof(DiscordInvite.Inviter))]
    public static partial DiscordInvite ToEntity(this DiscordInviteDto dto);
}
