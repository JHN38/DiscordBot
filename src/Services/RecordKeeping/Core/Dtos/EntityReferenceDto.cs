namespace DiscordBot.Service.RecordKeeping.Core.Dtos;

/// <summary>
/// Lightweight reference containing only the database ID and Discord ID.
/// Used for returning entity references from GetOrAdd operations without loading full entities.
/// </summary>
public sealed record EntityReferenceDto(int Id, ulong DiscordId);

/// <summary>
/// Contains resolved entity references after GetOrAdd operations complete.
/// Used to establish foreign key relationships without loading navigation properties.
/// </summary>
public sealed record MessageDependenciesDto(
    EntityReferenceDto Author,
    EntityReferenceDto Channel,
    EntityReferenceDto? Guild);
