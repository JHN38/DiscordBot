using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles UpdateMessageCommand by updating message content and edit timestamp.
/// </summary>
public sealed class UpdateMessageHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(UpdateMessageCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        var message = await db.Messages
            .FirstOrDefaultAsync(m => m.DiscordId == command.MessageId, cancellationToken);

        if (message is not null)
        {
            message.Content = command.Content;
            message.IsEdited = true;
            message.EditedTimestamp = command.EditedTimestamp;
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}

/// <summary>
/// Handles DeleteMessageCommand by soft-deleting the message.
/// </summary>
public sealed class DeleteMessageHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(DeleteMessageCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        var message = await db.Messages
            .FirstOrDefaultAsync(m => m.DiscordId == command.MessageId, cancellationToken);

        if (message is not null)
        {
            message.IsDeleted = true;
            message.DeletedAt = command.DeletedAt;
            await db.SaveChangesAsync(cancellationToken);
        }
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
