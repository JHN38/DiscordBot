using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordUser entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordUserMapper
{
    /// <summary>
    /// Maps a DiscordUserDto to a new DiscordUser entity.
    /// Navigation properties and audit fields are not set by this mapper.
    /// </summary>
    [MapperIgnoreTarget(nameof(DiscordUser.Id))]
    [MapperIgnoreTarget(nameof(DiscordUser.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordUser.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordUser.Messages))]
    [MapperIgnoreTarget(nameof(DiscordUser.Channels))]
    [MapperIgnoreTarget(nameof(DiscordUser.Guilds))]
    [MapperIgnoreTarget(nameof(DiscordUser.BannerUrl))]
    [MapperIgnoreTarget(nameof(DiscordUser.AccentColor))]
    [MapperIgnoreTarget(nameof(DiscordUser.IsSystem))]
    [MapperIgnoreTarget(nameof(DiscordUser.IsWebhook))]
    [MapperIgnoreTarget(nameof(DiscordUser.PublicFlags))]
    [MapperIgnoreTarget(nameof(DiscordUser.PremiumType))]
    [MapperIgnoreTarget(nameof(DiscordUser.Locale))]
    [MapperIgnoreTarget(nameof(DiscordUser.Reactions))]
    [MapperIgnoreTarget(nameof(DiscordUser.VoiceSessions))]
    [MapperIgnoreTarget(nameof(DiscordUser.PresenceLogs))]
    [MapperIgnoreTarget(nameof(DiscordUser.Bans))]
    [MapperIgnoreTarget(nameof(DiscordUser.BansIssued))]
    [MapperIgnoreTarget(nameof(DiscordUser.Nicknames))]
    [MapperIgnoreTarget(nameof(DiscordUser.Roles))]
    [MapperIgnoreTarget(nameof(DiscordUser.InvitesCreated))]
    [MapperIgnoreTarget(nameof(DiscordUser.ScheduledEventsCreated))]
    [MapperIgnoreTarget(nameof(DiscordUser.StickersCreated))]
    [MapperIgnoreTarget(nameof(DiscordUser.AuditLogsPerformed))]
    public static partial DiscordUser ToEntity(this DiscordUserDto dto);

    /// <summary>
    /// Maps a DiscordUser entity to a DiscordUserDto.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordUser.Id))]
    [MapperIgnoreSource(nameof(DiscordUser.CreatedOn))]
    [MapperIgnoreSource(nameof(DiscordUser.ModifiedOn))]
    [MapperIgnoreSource(nameof(DiscordUser.Messages))]
    [MapperIgnoreSource(nameof(DiscordUser.Channels))]
    [MapperIgnoreSource(nameof(DiscordUser.Guilds))]
    [MapperIgnoreSource(nameof(DiscordUser.BannerUrl))]
    [MapperIgnoreSource(nameof(DiscordUser.AccentColor))]
    [MapperIgnoreSource(nameof(DiscordUser.IsSystem))]
    [MapperIgnoreSource(nameof(DiscordUser.IsWebhook))]
    [MapperIgnoreSource(nameof(DiscordUser.PublicFlags))]
    [MapperIgnoreSource(nameof(DiscordUser.PremiumType))]
    [MapperIgnoreSource(nameof(DiscordUser.Locale))]
    [MapperIgnoreSource(nameof(DiscordUser.Reactions))]
    [MapperIgnoreSource(nameof(DiscordUser.VoiceSessions))]
    [MapperIgnoreSource(nameof(DiscordUser.PresenceLogs))]
    [MapperIgnoreSource(nameof(DiscordUser.Bans))]
    [MapperIgnoreSource(nameof(DiscordUser.BansIssued))]
    [MapperIgnoreSource(nameof(DiscordUser.Nicknames))]
    [MapperIgnoreSource(nameof(DiscordUser.Roles))]
    [MapperIgnoreSource(nameof(DiscordUser.InvitesCreated))]
    [MapperIgnoreSource(nameof(DiscordUser.ScheduledEventsCreated))]
    [MapperIgnoreSource(nameof(DiscordUser.StickersCreated))]
    [MapperIgnoreSource(nameof(DiscordUser.AuditLogsPerformed))]
    public static partial DiscordUserDto ToDto(this DiscordUser entity);
}
