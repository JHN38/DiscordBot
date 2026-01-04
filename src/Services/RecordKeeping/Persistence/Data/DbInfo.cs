using System.Text.Json;
using DiscordBot.Service.RecordKeeping.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Service.RecordKeeping.Persistence.Data;

public sealed class DbInfo(AppDbContext context) : IDbInfo
{
    private static readonly JsonSerializerOptions DefaultOptions = new() { WriteIndented = true };

    public Task<string> GetDatabaseSchemaAsync(JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        // Model.GetEntityTypes() is synchronous - no need for Task.Run
        var schema = context.Model.GetEntityTypes()
            .Select(entity => new
            {
                Table = entity.GetTableName(),
                Columns = entity.GetProperties().Select(p => p.Name).ToList()
            }).ToList();
        return Task.FromResult(JsonSerializer.Serialize(schema, options ?? DefaultOptions));
    }
}
