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
/// </summary>
public sealed class RemoveReactionHandler(IDbContextFactory<AppDbContext> factory)
{
    public async Task Handle(RemoveReactionCommand command, CancellationToken cancellationToken = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellationToken);

        // Find message and user IDs first
        var messageId = await db.Messages
            .Where(m => m.DiscordId == command.MessageId)
            .Select(m => (int?)m.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var userId = await db.Users
            .Where(u => u.DiscordId == command.UserId)
            .Select(u => (int?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!messageId.HasValue || !userId.HasValue)
            return;

        var reaction = await db.Reactions
            .FirstOrDefaultAsync(r =>
                r.MessageId == messageId.Value &&
                r.UserId == userId.Value &&
                r.EmojiName == command.EmojiName &&
                r.EmojiId == command.EmojiId,
                cancellationToken);

        if (reaction is not null)
        {
            reaction.RemovedAt = command.RemovedAt;
            await db.SaveChangesAsync(cancellationToken);
        }
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
