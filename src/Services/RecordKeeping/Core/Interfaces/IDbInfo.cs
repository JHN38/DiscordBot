using System.Text.Json;

namespace DiscordBot.Service.RecordKeeping.Core.Interfaces;

public interface IDbInfo
{
    Task<string> GetDatabaseSchemaAsync(JsonSerializerOptions? options = null, CancellationToken cancellationToken = default);
}
