using System.Data;
using System.Text.Json;
using DiscordBot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;

namespace DiscordBot.Infrastructure.Services.Chat;
public class ChatCompleteService(ILogger<ChatCompleteService> logger, OpenAIClient client, AppDbContext context)
{
    private readonly ILogger<ChatCompleteService> _logger = logger;
    private readonly OpenAIClient _client = client;
    private readonly AppDbContext _context = context;
    private readonly ChatTool _schemaTool = ChatTool.CreateFunctionTool(
        functionName: nameof(GetDatabaseSchemaAsync),
        functionDescription: "Retrieve the database schema for generating queries."
    );

    private readonly ChatTool _executeSQLTool = ChatTool.CreateFunctionTool(
        functionName: nameof(ExecuteSQLAsync),
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
        """u8.ToArray()), functionSchemaIsStrict: true
    );

    // Initialize the conversation messages list with the system prompt
    private readonly List<ChatMessage> _messages =
    [
        new SystemChatMessage("""
            You are a helpful assistant that helps the user by generating SQL queries when needed, executing them, and presenting the results to the user in plain text.
            When the user asks for information, generate the appropriate SQL query, use the provided tools to execute it, and then summarize the results in a clear and concise manner.
            """)
    ];

    public async Task<string> ProcessMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        ChatCompletionOptions options = new()
        {
            Tools = { _schemaTool, _executeSQLTool },
            // ResponseFormat is removed to allow free-form responses
        };

        // Add the user's message to the conversation history
        _messages.Add(new UserChatMessage(message));

        int maxHop = 10;
        do
        {
            ChatCompletion completion = await _client.GetChatClient("gpt-4o").CompleteChatAsync(_messages, options, cancellationToken);

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    // Assistant finished the response.
                    _messages.Add(new AssistantChatMessage(completion));

                    // Extract the assistant's response text
                    _logger.LogDebug("[Stop] Assistant Response: {AssistantResponse}",
                        JsonSerializer.Serialize(_messages[^1].Content.Select(c => c.Text), options: new() { WriteIndented = true }));

                    return string.Join(Environment.NewLine, _messages[^1].Content.Select(c => c.Text));

                case ChatFinishReason.ToolCalls:
                    // Handle tool calls.
                    _logger.LogDebug("[ToolCalls] Tool Calls Detected: {ToolCalls}",
                        JsonSerializer.Serialize(completion.ToolCalls, options: new() { WriteIndented = true }));

                    // Add the tool call request to the conversation history.
                    _messages.Add(new AssistantChatMessage(completion));

                    // Handle tool calls.
                    foreach (ChatToolCall toolCall in completion.ToolCalls)
                    {
                        _logger.LogDebug("Tool: {FunctionName}\nArguments: {FunctionArguments}",
                            toolCall.FunctionName, toolCall.FunctionArguments);

                        switch (toolCall.FunctionName)
                        {
                            case nameof(GetDatabaseSchemaAsync):
                                {
                                    string schema = await GetDatabaseSchemaAsync(cancellationToken);
                                    _messages.Add(new ToolChatMessage(toolCall.Id, schema));
                                    break;
                                }

                            case nameof(ExecuteSQLAsync):
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
                                    _messages.Add(new ToolChatMessage(toolCall.Id, result));
                                    break;
                                }
                            default:
                                _logger.LogWarning("Unhandled Tool Call: {FunctionName}", toolCall.FunctionName);
                                throw new NotImplementedException($"Unknown tool: {toolCall.FunctionName}");
                        }
                    }

                    break;

                default:
                    _logger.LogWarning("Unexpected Finish Reason: {FinishReason}", completion.FinishReason);
                    throw new NotImplementedException(completion.FinishReason.ToString());
            }
        } while (maxHop-- > 0);

        _logger.LogWarning("Unexpected end to {ProcessMethod}", nameof(ProcessMessageAsync));
        throw new NotImplementedException(nameof(ProcessMessageAsync));
    }

    /// <inheritdoc />
    public Task<string> GetDatabaseSchemaAsync(CancellationToken cancellationToken = default) =>
    Task.Run(() =>
    {
        var schema = _context.Model.GetEntityTypes()
            .Select(entity => new
            {
                Table = entity.GetTableName(),
                Columns = entity.GetProperties().Select(p => p.Name).ToList()
            }).ToList();
        return JsonSerializer.Serialize(schema);
    }, cancellationToken);

    async Task<string> ExecuteSQLAsync(string query, IDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("SQL query cannot be empty or whitespace.", nameof(query));

        _logger.LogDebug("Query Result:\n{Query}", query);

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
