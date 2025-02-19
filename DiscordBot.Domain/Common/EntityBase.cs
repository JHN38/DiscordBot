// This code is based on the BaseEntity class from the CleanArchitecture project by jasontaylordev.
// You can find the original code here: https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/Domain/Common/BaseEntity.cs
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Domain.Common;

/// <summary>
/// Represents the base entity with a strongly typed identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class EntityBase<TId> where TId : struct
{
    /// <summary>
    /// Gets or sets the identifier of the entity.
    /// </summary>
    [MapperIgnore]
    public TId Id { get; set; } = default!;
}
