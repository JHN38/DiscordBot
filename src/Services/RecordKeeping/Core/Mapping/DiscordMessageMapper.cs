using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordMessage entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordMessageMapper
{
    /// <summary>
    /// Maps a DiscordMessageDto to a new DiscordMessage entity.
    /// Navigation properties (Author, Channel, Guild, ReferencedMessage) must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordMessageDto.Author))]
    [MapperIgnoreSource(nameof(DiscordMessageDto.Channel))]
    [MapperIgnoreSource(nameof(DiscordMessageDto.ReferencedMessageDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordMessage.Id))]
    [MapperIgnoreTarget(nameof(DiscordMessage.CreatedOn))]
    [MapperIgnoreTarget(nameof(DiscordMessage.ModifiedOn))]
    [MapperIgnoreTarget(nameof(DiscordMessage.Author))]
    [MapperIgnoreTarget(nameof(DiscordMessage.Channel))]
    [MapperIgnoreTarget(nameof(DiscordMessage.Guild))]
    [MapperIgnoreTarget(nameof(DiscordMessage.ReferencedMessage))]
    [MapperIgnoreTarget(nameof(DiscordMessage.ReferencedByMessages))]
    [MapperIgnoreTarget(nameof(DiscordMessage.MessageType))]
    [MapperIgnoreTarget(nameof(DiscordMessage.IsPinned))]
    [MapperIgnoreTarget(nameof(DiscordMessage.IsTTS))]
    [MapperIgnoreTarget(nameof(DiscordMessage.MentionedEveryone))]
    [MapperIgnoreTarget(nameof(DiscordMessage.MentionedUserIds))]
    [MapperIgnoreTarget(nameof(DiscordMessage.MentionedRoleIds))]
    [MapperIgnoreTarget(nameof(DiscordMessage.MentionedChannelIds))]
    [MapperIgnoreTarget(nameof(DiscordMessage.Flags))]
    [MapperIgnoreTarget(nameof(DiscordMessage.ApplicationId))]
    [MapperIgnoreTarget(nameof(DiscordMessage.WebhookId))]
    [MapperIgnoreTarget(nameof(DiscordMessage.InteractionId))]
    [MapperIgnoreTarget(nameof(DiscordMessage.ThreadId))]
    [MapperIgnoreTarget(nameof(DiscordMessage.IsDeleted))]
    [MapperIgnoreTarget(nameof(DiscordMessage.DeletedAt))]
    [MapperIgnoreTarget(nameof(DiscordMessage.Reactions))]
    [MapperIgnoreTarget(nameof(DiscordMessage.Attachments))]
    [MapperIgnoreTarget(nameof(DiscordMessage.Embeds))]
    [MapperIgnoreTarget(nameof(DiscordMessage.Stickers))]
    public static partial DiscordMessage ToEntity(this DiscordMessageDto dto);
}
