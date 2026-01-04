using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordChannel entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordChannelMapper
{
    /// <summary>
    /// Maps a DiscordChannelDto to a new DiscordChannel entity.
    /// The Guild navigation property must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordChannelDto.Guild))]
    [MapperIgnoreTarget(nameof(DiscordChannel.Id))]
    [MapperIgnoreTarget(nameof(DiscordChannel.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordChannel.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordChannel.Guild))]
    [MapperIgnoreTarget(nameof(DiscordChannel.Messages))]
    [MapperIgnoreTarget(nameof(DiscordChannel.Users))]
    [MapperIgnoreTarget(nameof(DiscordChannel.ChannelType))]
    [MapperIgnoreTarget(nameof(DiscordChannel.Topic))]
    [MapperIgnoreTarget(nameof(DiscordChannel.Position))]
    [MapperIgnoreTarget(nameof(DiscordChannel.IsNsfw))]
    [MapperIgnoreTarget(nameof(DiscordChannel.SlowModeInterval))]
    [MapperIgnoreTarget(nameof(DiscordChannel.ParentId))]
    [MapperIgnoreTarget(nameof(DiscordChannel.Bitrate))]
    [MapperIgnoreTarget(nameof(DiscordChannel.UserLimit))]
    [MapperIgnoreTarget(nameof(DiscordChannel.RateLimitPerUser))]
    [MapperIgnoreTarget(nameof(DiscordChannel.PermissionOverwrites))]
    [MapperIgnoreTarget(nameof(DiscordChannel.DefaultAutoArchiveDuration))]
    [MapperIgnoreTarget(nameof(DiscordChannel.IsArchived))]
    [MapperIgnoreTarget(nameof(DiscordChannel.ArchiveTimestamp))]
    [MapperIgnoreTarget(nameof(DiscordChannel.IsLocked))]
    [MapperIgnoreTarget(nameof(DiscordChannel.DefaultThreadRateLimitPerUser))]
    [MapperIgnoreTarget(nameof(DiscordChannel.AvailableTags))]
    [MapperIgnoreTarget(nameof(DiscordChannel.DefaultReactionEmoji))]
    [MapperIgnoreTarget(nameof(DiscordChannel.DefaultSortOrder))]
    [MapperIgnoreTarget(nameof(DiscordChannel.IsDeleted))]
    [MapperIgnoreTarget(nameof(DiscordChannel.DeletedAt))]
    [MapperIgnoreTarget(nameof(DiscordChannel.CategoryId))]
    [MapperIgnoreTarget(nameof(DiscordChannel.Category))]
    [MapperIgnoreTarget(nameof(DiscordChannel.ChildChannels))]
    [MapperIgnoreTarget(nameof(DiscordChannel.VoiceSessions))]
    [MapperIgnoreTarget(nameof(DiscordChannel.Invites))]
    [MapperIgnoreTarget(nameof(DiscordChannel.ScheduledEvents))]
    public static partial DiscordChannel ToEntity(this DiscordChannelDto dto);

    /// <summary>
    /// Maps a DiscordChannel entity to a DiscordChannelDto.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordChannel.Id))]
    [MapperIgnoreSource(nameof(DiscordChannel.CreatedOn))]
    [MapperIgnoreSource(nameof(DiscordChannel.ModifiedOn))]
    [MapperIgnoreSource(nameof(DiscordChannel.Guild))]
    [MapperIgnoreSource(nameof(DiscordChannel.Messages))]
    [MapperIgnoreSource(nameof(DiscordChannel.Users))]
    [MapperIgnoreSource(nameof(DiscordChannel.ChannelType))]
    [MapperIgnoreSource(nameof(DiscordChannel.Topic))]
    [MapperIgnoreSource(nameof(DiscordChannel.Position))]
    [MapperIgnoreSource(nameof(DiscordChannel.IsNsfw))]
    [MapperIgnoreSource(nameof(DiscordChannel.SlowModeInterval))]
    [MapperIgnoreSource(nameof(DiscordChannel.ParentId))]
    [MapperIgnoreSource(nameof(DiscordChannel.Bitrate))]
    [MapperIgnoreSource(nameof(DiscordChannel.UserLimit))]
    [MapperIgnoreSource(nameof(DiscordChannel.RateLimitPerUser))]
    [MapperIgnoreSource(nameof(DiscordChannel.PermissionOverwrites))]
    [MapperIgnoreSource(nameof(DiscordChannel.DefaultAutoArchiveDuration))]
    [MapperIgnoreSource(nameof(DiscordChannel.IsArchived))]
    [MapperIgnoreSource(nameof(DiscordChannel.ArchiveTimestamp))]
    [MapperIgnoreSource(nameof(DiscordChannel.IsLocked))]
    [MapperIgnoreSource(nameof(DiscordChannel.DefaultThreadRateLimitPerUser))]
    [MapperIgnoreSource(nameof(DiscordChannel.AvailableTags))]
    [MapperIgnoreSource(nameof(DiscordChannel.DefaultReactionEmoji))]
    [MapperIgnoreSource(nameof(DiscordChannel.DefaultSortOrder))]
    [MapperIgnoreSource(nameof(DiscordChannel.IsDeleted))]
    [MapperIgnoreSource(nameof(DiscordChannel.DeletedAt))]
    [MapperIgnoreSource(nameof(DiscordChannel.CategoryId))]
    [MapperIgnoreSource(nameof(DiscordChannel.Category))]
    [MapperIgnoreSource(nameof(DiscordChannel.ChildChannels))]
    [MapperIgnoreSource(nameof(DiscordChannel.VoiceSessions))]
    [MapperIgnoreSource(nameof(DiscordChannel.Invites))]
    [MapperIgnoreSource(nameof(DiscordChannel.ScheduledEvents))]
    [MapperIgnoreTarget(nameof(DiscordChannelDto.Guild))]
    public static partial DiscordChannelDto ToDto(this DiscordChannel entity);
}
