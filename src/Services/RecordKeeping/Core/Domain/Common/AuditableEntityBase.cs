namespace DiscordBot.Service.RecordKeeping.Core.Domain.Common;

public abstract class AuditableEntityBase<TId> : EntityBase<TId>, IAuditableEntity
    where TId : struct
{
    public DateTimeOffset CreatedOn { get; set; }

    public DateTimeOffset? ModifiedOn { get; set; }
}
