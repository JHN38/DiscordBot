using System.Data;
using System.Text.Json;
using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Application.Interfaces;
using DiscordBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace DiscordBot.Infrastructure.Services;

public class ChatToolFactory(AppDbContext context, IDbInfo dbInfo, ILogger<ChatToolFactory> logger) : IChatToolFactory
{
    private readonly AppDbContext _context = context;
    private readonly IDbInfo _dbInfo = dbInfo;
    private readonly ILogger<ChatToolFactory> _logger = logger;

    private readonly ChatTool _schemaTool = ChatTool.CreateFunctionTool(
        functionName: "GetDatabaseSchema",
        functionDescription: "Retrieve the database schema for generating queries."
    );
    private readonly ChatTool _executeSQLTool = ChatTool.CreateFunctionTool(
        functionName: "ExecuteSQL",
        functionDescription: "Execute a parameterized SQL query against the database.",
        functionParameters: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "query": {
                                "type": "string",
                                "description": "The SQL query to execute. Use parameterized queries for safety. For example, 'SELECT * FROM Users WHERE Age > @Age'."
                            },
                            "parameters": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "name": {
                                            "type": "string",
                                            "description": "The name of the SQL parameter, including the '@' symbol."
                                        },
                                        "value": {
                                            "type": ["string", "number", "boolean", "null"],
                                            "description": "The value of the parameter."
                                        }
                                    },
                                    "required": ["name", "value"],
                                    "additionalProperties": false
                                },
                                "description": "The list of SQL parameters as objects with 'name' and 'value' fields."
                            }
                        },
                        "required": ["query", "parameters"],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
        functionSchemaIsStrict: true
    );

    public IEnumerable<ChatTool> GetTools() =>
        [_schemaTool, _executeSQLTool];

    public async Task<string> ExecuteToolAsync(ChatToolCall toolCall, CancellationToken cancellationToken = default)
    {
        switch (toolCall.FunctionName)
        {
            case "GetDatabaseSchema":
                {
                    string schema = await _dbInfo.GetDatabaseSchemaAsync(cancellationToken);
                    return schema;
                }

            case "ExecuteSQL":
                {
                    using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);

                    if (!argumentsJson.RootElement.TryGetProperty("query", out JsonElement queryElement) ||
                        queryElement.GetString() is not string query)
                        throw new ArgumentException("Query is required.");

                    var parameters = argumentsJson.RootElement.TryGetProperty("parameters", out JsonElement paramsElement)
                        ? paramsElement.EnumerateArray()
                            .Select(p =>
                                p.TryGetProperty("name", out var nameProp) &&
                                p.TryGetProperty("value", out var valueProp)
                                    ? new { Name = nameProp.GetString(), Value = GetJsonValue(valueProp) }
                                    : null)
                            .Where(x => x != null && x.Name != null && x.Value != null) // Filter invalid entries
                            .ToDictionary(
                                x => x!.Name!, // Nulls are filtered out
                                x => x?.Value
                            )
                        : null;

                    // Helper method to extract the value as an object
                    static object? GetJsonValue(JsonElement element) =>
                        element.ValueKind switch
                        {
                            JsonValueKind.String => element.GetString(),
                            JsonValueKind.Number =>
                                element.TryGetInt32(out var intValue) ? intValue :
                                element.TryGetInt64(out var longValue) ? longValue :
                                element.TryGetDecimal(out var decimalValue) ? decimalValue :
                                element.TryGetDouble(out var doubleValue) ? doubleValue :
                                element.GetRawText(),
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.Null => null,
                            _ => element.GetRawText() // Fallback to raw JSON for complex types like objects/arrays
                        };

                    string result = await ExecuteSQLAsync(query, parameters, cancellationToken);
                    return result;
                }
            default:
                _logger.LogWarning("Unhandled Tool Call: {FunctionName}", toolCall.FunctionName);
                throw new NotImplementedException($"Unknown tool: {toolCall.FunctionName}");
        }
    }


    async Task<string> ExecuteSQLAsync(string query, IDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("SQL query cannot be empty or whitespace.", nameof(query));

        _logger.LogDebug("Executing Query:\n{Query}", query);

        using var connection = _context.Database.GetDbConnection();

        // Ensure the connection is open
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        using var command = connection.CreateCommand();
        command.CommandText = query;
        command.CommandType = CommandType.Text;

        // Add parameters if provided
        if (parameters != null)
        {
            var parametersJoined = string.Join(", ", parameters!.Select(p => $"{{{p.Key}='{p.Value}'}}"));
            _logger.LogDebug("Parameters=[{ParametersJoined}]", parametersJoined);

            foreach (var param in parameters)
            {
                var dbParam = command.CreateParameter();
                dbParam.ParameterName = param.Key;
                dbParam.Value = param.Value ?? DBNull.Value;
                command.Parameters.Add(dbParam);
            }
        }

        var results = new List<Dictionary<string, object>>();

        // Execute and process results
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(
                Enumerable.Range(0, reader.FieldCount)
                          .ToDictionary(reader.GetName, reader.GetValue)
            );
        }

        _logger.LogDebug("Query Result:\n{QueryResult}",
            JsonSerializer.Serialize(results, options: new() { WriteIndented = true }));

        // Serialize results to JSON
        return JsonSerializer.Serialize(results);
    }
}


public static class JsonValueKindExtensions
{
    public static DbType ToDbType(this JsonValueKind valueKind) =>
        valueKind switch
        {
            JsonValueKind.String => DbType.String,
            JsonValueKind.Number => DbType.Decimal, // DbType.Decimal is a generic choice for numbers
            JsonValueKind.True => DbType.Boolean,
            JsonValueKind.False => DbType.Boolean,
            JsonValueKind.Null => DbType.Object, // Represents a nullable database type
            JsonValueKind.Array => DbType.Object, // Arrays typically require custom handling
            JsonValueKind.Object => DbType.Object, // JSON objects are usually serialized into strings or blobs
            _ => throw new NotSupportedException($"Unsupported JsonValueKind: {valueKind}")
        };
}
