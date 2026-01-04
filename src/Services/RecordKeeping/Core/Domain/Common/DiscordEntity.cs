namespace DiscordBot.Service.RecordKeeping.Core.Domain.Common;

public abstract class DiscordEntity : AuditableEntityBase<int>
{
    public ulong DiscordId { get; set; }
}
