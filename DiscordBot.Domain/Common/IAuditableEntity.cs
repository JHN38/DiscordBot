using Riok.Mapperly.Abstractions;

namespace DiscordBot.Domain.Common;

public interface IAuditableEntity
{
    [MapperIgnore]
    public DateTimeOffset CreatedOn { get; set; }

    [MapperIgnore]
    public DateTimeOffset? ModifiedOn { get; set; }
}
