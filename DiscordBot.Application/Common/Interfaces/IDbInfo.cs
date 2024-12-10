using System.Text.Json;

namespace DiscordBot.Application.Common.Interfaces;

public interface IDbInfo
{
    Task<string> GetDatabaseSchemaAsync(CancellationToken cancellationToken = default);
    Task<string> GetDatabaseSchemaAsync(JsonSerializerOptions options, CancellationToken cancellationToken = default);
}
