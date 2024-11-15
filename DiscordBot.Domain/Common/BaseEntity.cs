// This code is based on the BaseEntity class from the CleanArchitecture project by jasontaylordev.
// You can find the original code here: https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/Domain/Common/BaseEntity.cs
using System.ComponentModel.DataAnnotations.Schema;
using Riok.Mapperly.Abstractions;

namespace DiscordBot.Domain.Common;

/// <summary>
/// Represents the base entity with a strongly typed identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class BaseEntity<TId> where TId : struct
{
    /// <summary>
    /// Gets or sets the identifier of the entity.
    /// </summary>
    [MapperIgnore]
    public TId Id { get; set; } = default!;

    private readonly List<BaseEvent> _domainEvents = [];

    /// <summary>
    /// Gets the domain events associated with the entity.
    /// </summary>
    [NotMapped]
    [MapperIgnore]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Removes a domain event from the entity.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);

    /// <summary>
    /// Clears all domain events from the entity.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
