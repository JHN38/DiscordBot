using DiscordBot.Contracts.RecordKeeping;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using DiscordBot.Service.RecordKeeping.Core.Mapping;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Service.RecordKeeping.Handlers;

/// <summary>
/// Handles SaveReactionCommand by delegating to IDiscordEntityManager.
/// </summary>
public sealed class SaveReactionHandler(IDiscordEntityManager entityManager)
{
    public async Task Handle(SaveReactionCommand command, CancellationToken cancellationToken = default)
    {
        var reactionDto = command.ToReactionDto();
        await entityManager.GetOrAddReactionAsync(reactionDto, cancellationToken);
    }
}

/// <summary>
/// Handles RemoveReactionCommand by soft-deleting the reaction.
/// Uses atomic ExecuteUpdateAsync for safe concurrent processing.
/// </summary>
public sealed class RemoveReactionHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(RemoveReactionCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Use subqueries to resolve FKs atomically within the update
        var messageIds = db.Messages
            .Where(m => m.DiscordId == command.MessageId)
            .Select(m => m.Id);

        var userIds = db.Users
            .Where(u => u.DiscordId == command.UserId)
            .Select(u => u.Id);

        await db.Reactions
            .Where(r =>
                messageIds.Contains(r.MessageId) &&
                userIds.Contains(r.UserId) &&
                r.EmojiName == command.EmojiName &&
                r.EmojiId == command.EmojiId &&
                r.RemovedAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.RemovedAt, command.RemovedAt),
                cancellationToken);
    }
}

/// <summary>
/// Handles ClearReactionsCommand by soft-deleting all reactions on a message.
/// </summary>
public sealed class ClearReactionsHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(ClearReactionsCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        var messageId = await db.Messages
            .Where(m => m.DiscordId == command.MessageId)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!messageId.HasValue)
            return;

        await db.Reactions
            .Where(r => r.MessageId == messageId.Value && r.RemovedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.RemovedAt, command.RemovedAt), cancellationToken);
    }
}

/// <summary>
/// Handles RemoveEmoteReactionsCommand by soft-deleting all reactions of a specific emote.
/// </summary>
public sealed class RemoveEmoteReactionsHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(RemoveEmoteReactionsCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        var messageId = await db.Messages
            .Where(m => m.DiscordId == command.MessageId)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!messageId.HasValue)
            return;

        await db.Reactions
            .Where(r =>
                r.MessageId == messageId.Value &&
                r.EmojiName == command.EmojiName &&
                r.EmojiId == command.EmojiId &&
                r.RemovedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.RemovedAt, command.RemovedAt), cancellationToken);
    }
}
