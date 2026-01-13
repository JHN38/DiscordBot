using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles UpdateMessageCommand by updating message content and edit timestamp.
/// Uses atomic ExecuteUpdateAsync for safe concurrent processing.
/// </summary>
public sealed class UpdateMessageHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(UpdateMessageCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        await db.Messages
            .Where(m => m.DiscordId == command.MessageId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.Content, command.Content)
                .SetProperty(m => m.IsEdited, true)
                .SetProperty(m => m.EditedTimestamp, command.EditedTimestamp),
                cancellationToken);
    }
}

/// <summary>
/// Handles DeleteMessageCommand by soft-deleting the message.
/// Uses atomic ExecuteUpdateAsync for safe concurrent processing.
/// </summary>
public sealed class DeleteMessageHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(DeleteMessageCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        await db.Messages
            .Where(m => m.DiscordId == command.MessageId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.IsDeleted, true)
                .SetProperty(m => m.DeletedAt, command.DeletedAt),
                cancellationToken);
    }
}

/// <summary>
/// Handles BulkDeleteMessagesCommand by soft-deleting multiple messages.
/// </summary>
public sealed class BulkDeleteMessagesHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(BulkDeleteMessagesCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        await db.Messages
            .Where(m => command.MessageIds.Contains(m.DiscordId))
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.IsDeleted, true)
                .SetProperty(m => m.DeletedAt, command.DeletedAt),
                cancellationToken);
    }
}
