namespace DiscordBot.Service.RecordKeeping.Core.Domain.Common;

public interface IAuditableEntity
{
    DateTimeOffset CreatedOn { get; set; }

    DateTimeOffset? ModifiedOn { get; set; }
}
