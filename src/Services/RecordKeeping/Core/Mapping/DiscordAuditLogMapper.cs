using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using DiscordBot.Service.RecordKeeping.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Service.RecordKeeping.Core.Mapping;

/// <summary>
/// Source-generated mapper for DiscordAuditLog entity.
/// Uses Riok.Mapperly for compile-time mapping without runtime reflection.
/// </summary>
[Mapper]
public static partial class DiscordAuditLogMapper
{
    /// <summary>
    /// Maps a DiscordAuditLogDto to a new DiscordAuditLog entity.
    /// Navigation properties must be set separately.
    /// </summary>
    [MapperIgnoreSource(nameof(DiscordAuditLogDto.GuildDiscordId))]
    [MapperIgnoreSource(nameof(DiscordAuditLogDto.PerformedByDiscordId))]
    [MapperIgnoreTarget(nameof(DiscordAuditLog.Id))]
    [MapperIgnoreTarget(nameof(DiscordAuditLog.GuildId))]
    [MapperIgnoreTarget(nameof(DiscordAuditLog.Guild))]
    [MapperIgnoreTarget(nameof(DiscordAuditLog.PerformedById))]
    [MapperIgnoreTarget(nameof(DiscordAuditLog.PerformedBy))]
    public static partial DiscordAuditLog ToEntity(this DiscordAuditLogDto dto);
}
