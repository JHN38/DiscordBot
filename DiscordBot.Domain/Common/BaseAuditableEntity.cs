// This code is based on the BaseAuditableEntity class from the CleanArchitecture project by jasontaylordev.
// You can find the original code here: https://github.com/jasontaylordev/CleanArchitecture/blob/main/src/Domain/Common/BaseAuditableEntity.cs
namespace DiscordBot.Domain.Common;

/// <summary>
/// Represents a base auditable entity with common properties for tracking creation and modification.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity
    where TId : struct
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified.
    /// </summary>
    public DateTimeOffset? ModifiedOn { get; set; }
}
