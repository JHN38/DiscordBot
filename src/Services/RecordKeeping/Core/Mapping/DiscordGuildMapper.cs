using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordGuild entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordGuildMapper
{
    /// <summary>
    /// Maps a DiscordGuildDto to a new DiscordGuild entity.
    /// Navigation properties and audit fields are not set by this mapper.
    /// </summary>
    [MapperIgnoreTarget(nameof(DiscordGuild.Id))]
    [MapperIgnoreTarget(nameof(DiscordGuild.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordGuild.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordGuild.Channels))]
    [MapperIgnoreTarget(nameof(DiscordGuild.Users))]
    [MapperIgnoreTarget(nameof(DiscordGuild.Messages))]
    [MapperIgnoreTarget(nameof(DiscordGuild.Description))]
    [MapperIgnoreTarget(nameof(DiscordGuild.IconUrl))]
    [MapperIgnoreTarget(nameof(DiscordGuild.BannerUrl))]
    [MapperIgnoreTarget(nameof(DiscordGuild.OwnerId))]
    [MapperIgnoreTarget(nameof(DiscordGuild.MemberCount))]
    [MapperIgnoreTarget(nameof(DiscordGuild.PremiumTier))]
    [MapperIgnoreTarget(nameof(DiscordGuild.PremiumSubscriptionCount))]
    [MapperIgnoreTarget(nameof(DiscordGuild.PreferredLocale))]
    [MapperIgnoreTarget(nameof(DiscordGuild.VanityUrlCode))]
    [MapperIgnoreTarget(nameof(DiscordGuild.IsNsfw))]
    [MapperIgnoreTarget(nameof(DiscordGuild.VerificationLevel))]
    [MapperIgnoreTarget(nameof(DiscordGuild.DefaultMessageNotifications))]
    [MapperIgnoreTarget(nameof(DiscordGuild.ExplicitContentFilter))]
    [MapperIgnoreTarget(nameof(DiscordGuild.SystemChannelId))]
    [MapperIgnoreTarget(nameof(DiscordGuild.RulesChannelId))]
    [MapperIgnoreTarget(nameof(DiscordGuild.PublicUpdatesChannelId))]
    [MapperIgnoreTarget(nameof(DiscordGuild.AfkChannelId))]
    [MapperIgnoreTarget(nameof(DiscordGuild.AfkTimeout))]
    [MapperIgnoreTarget(nameof(DiscordGuild.WidgetEnabled))]
    [MapperIgnoreTarget(nameof(DiscordGuild.Features))]
    [MapperIgnoreTarget(nameof(DiscordGuild.MaxMembers))]
    [MapperIgnoreTarget(nameof(DiscordGuild.MaxVideoChannelUsers))]
    [MapperIgnoreTarget(nameof(DiscordGuild.ApproximateMemberCount))]
    [MapperIgnoreTarget(nameof(DiscordGuild.ApproximatePresenceCount))]
    [MapperIgnoreTarget(nameof(DiscordGuild.JoinedAt))]
    [MapperIgnoreTarget(nameof(DiscordGuild.IsAvailable))]
    [MapperIgnoreTarget(nameof(DiscordGuild.Roles))]
    [MapperIgnoreTarget(nameof(DiscordGuild.Invites))]
    [MapperIgnoreTarget(nameof(DiscordGuild.ScheduledEvents))]
    [MapperIgnoreTarget(nameof(DiscordGuild.Bans))]
    [MapperIgnoreTarget(nameof(DiscordGuild.VoiceSessions))]
    [MapperIgnoreTarget(nameof(DiscordGuild.PresenceLogs))]
    [MapperIgnoreTarget(nameof(DiscordGuild.Nicknames))]
    [MapperIgnoreTarget(nameof(DiscordGuild.AuditLogs))]
    [MapperIgnoreTarget(nameof(DiscordGuild.Stickers))]
    public static partial DiscordGuild ToEntity(this DiscordGuildDto dto);

    /// <summary>
    /// Maps a DiscordGuild entity to a DiscordGuildDto.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordGuild.Id))]
    [MapperIgnoreSource(nameof(DiscordGuild.CreatedOn))]
    [MapperIgnoreSource(nameof(DiscordGuild.ModifiedOn))]
    [MapperIgnoreSource(nameof(DiscordGuild.Channels))]
    [MapperIgnoreSource(nameof(DiscordGuild.Users))]
    [MapperIgnoreSource(nameof(DiscordGuild.Messages))]
    [MapperIgnoreSource(nameof(DiscordGuild.Description))]
    [MapperIgnoreSource(nameof(DiscordGuild.IconUrl))]
    [MapperIgnoreSource(nameof(DiscordGuild.BannerUrl))]
    [MapperIgnoreSource(nameof(DiscordGuild.OwnerId))]
    [MapperIgnoreSource(nameof(DiscordGuild.MemberCount))]
    [MapperIgnoreSource(nameof(DiscordGuild.PremiumTier))]
    [MapperIgnoreSource(nameof(DiscordGuild.PremiumSubscriptionCount))]
    [MapperIgnoreSource(nameof(DiscordGuild.PreferredLocale))]
    [MapperIgnoreSource(nameof(DiscordGuild.VanityUrlCode))]
    [MapperIgnoreSource(nameof(DiscordGuild.IsNsfw))]
    [MapperIgnoreSource(nameof(DiscordGuild.VerificationLevel))]
    [MapperIgnoreSource(nameof(DiscordGuild.DefaultMessageNotifications))]
    [MapperIgnoreSource(nameof(DiscordGuild.ExplicitContentFilter))]
    [MapperIgnoreSource(nameof(DiscordGuild.SystemChannelId))]
    [MapperIgnoreSource(nameof(DiscordGuild.RulesChannelId))]
    [MapperIgnoreSource(nameof(DiscordGuild.PublicUpdatesChannelId))]
    [MapperIgnoreSource(nameof(DiscordGuild.AfkChannelId))]
    [MapperIgnoreSource(nameof(DiscordGuild.AfkTimeout))]
    [MapperIgnoreSource(nameof(DiscordGuild.WidgetEnabled))]
    [MapperIgnoreSource(nameof(DiscordGuild.Features))]
    [MapperIgnoreSource(nameof(DiscordGuild.MaxMembers))]
    [MapperIgnoreSource(nameof(DiscordGuild.MaxVideoChannelUsers))]
    [MapperIgnoreSource(nameof(DiscordGuild.ApproximateMemberCount))]
    [MapperIgnoreSource(nameof(DiscordGuild.ApproximatePresenceCount))]
    [MapperIgnoreSource(nameof(DiscordGuild.JoinedAt))]
    [MapperIgnoreSource(nameof(DiscordGuild.IsAvailable))]
    [MapperIgnoreSource(nameof(DiscordGuild.Roles))]
    [MapperIgnoreSource(nameof(DiscordGuild.Invites))]
    [MapperIgnoreSource(nameof(DiscordGuild.ScheduledEvents))]
    [MapperIgnoreSource(nameof(DiscordGuild.Bans))]
    [MapperIgnoreSource(nameof(DiscordGuild.VoiceSessions))]
    [MapperIgnoreSource(nameof(DiscordGuild.PresenceLogs))]
    [MapperIgnoreSource(nameof(DiscordGuild.Nicknames))]
    [MapperIgnoreSource(nameof(DiscordGuild.AuditLogs))]
    [MapperIgnoreSource(nameof(DiscordGuild.Stickers))]
    public static partial DiscordGuildDto ToDto(this DiscordGuild entity);
}
