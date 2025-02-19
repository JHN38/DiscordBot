using System.Text.Json;
using DiscordBot.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Infrastructure.Data;

public class DbInfo(AppDbContext context) : IDbInfo
{
    private readonly AppDbContext _context = context;

    /// <inheritdoc />
    public Task<string> GetDatabaseSchemaAsync(CancellationToken cancellationToken = default) =>
        GetDatabaseSchemaAsync(new() { WriteIndented = true }, cancellationToken);

    /// <inheritdoc />
    public Task<string> GetDatabaseSchemaAsync(JsonSerializerOptions options, CancellationToken cancellationToken = default) =>
        Task.Run(() =>
        {
            var schema = _context.Model.GetEntityTypes()
                .Select(entity => new
                {
                    Table = entity.GetTableName(),
                    Columns = entity.GetProperties().Select(p => p.Name).ToList()
                }).ToList();
            return JsonSerializer.Serialize(schema, options);
        }, cancellationToken);
}
