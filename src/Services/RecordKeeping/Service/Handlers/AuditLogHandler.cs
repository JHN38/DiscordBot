using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveAuditLogCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SaveAuditLogHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveAuditLogCommand command, CancellationToken cancellationToken = default)
    {
        var auditLogDto = command.ToAuditLogDto();
        await entityManager.AddAuditLogAsync(auditLogDto, cancellationToken);
    }
}
