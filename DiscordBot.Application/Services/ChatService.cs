// DiscordBot.Application/Services/ChatService.cs
using System.Text.Json;
using Discord;
using DiscordBot.Application.Interfaces;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace DiscordBot.Application.Services
{
    public class ChatService(
        ILogger<ChatService> logger,
        IChatCompletionService chatCompletionService,
        IChatToolFactory chatToolFactory) : IChatService
    {
        private readonly ILogger<ChatService> _logger = logger;
        private readonly IChatCompletionService _chatCompletionService = chatCompletionService;
        private readonly IChatToolFactory _chatToolFactory = chatToolFactory;

        private static string SystemMessage => $$"""
            You are a helpful assistant that helps the user by generating SQL queries when needed, executing them, and presenting the results to the user in plain text. 
            You have access to a SQLite database containing information about guilds, users, channels, and messages, and can use it to answer questions and provide insights.
            You also have access to the database schema as a tool. Whenever you need to determine the structure of tables, columns, and relationships, query the schema first to ensure the SQL queries you generate are accurate and align with the database's structure.

            ### Key Instructions:
            1. When a user requests information:
               - First, verify the database schema to understand the structure and relationships of tables if needed.
               - Generate a SQL query based on the schema and the user's query.
               - Execute the query and summarize the results conversationally.

            2. Use context clues to tailor your queries:
               - For example, when asked for "funny moments," search for keywords or emojis like `funny`, `😂`, or `lol` in the messages table.
               - When filtering by users, look up the user in the `users` table to get the correct `user_id`.

            3. Always confirm your understanding of the schema:
               - Use `PRAGMA table_info(table_name)` to get the structure of a table.
               - Use `SELECT name FROM sqlite_master WHERE type='table';` to list all tables.

            4. Make sure the query is **read-only**:
               - Only generate `SELECT` queries to retrieve data.
               - Do not generate queries that modify the database, such as `INSERT`, `UPDATE`, `DELETE`, 'DROP', or `ALTER`.
               - If the user's request implies modifying the database, politely explain that this is not allowed and suggest retrieving relevant data instead.

               Example of acceptable queries:
               - `SELECT message_content, timestamp FROM messages WHERE author_id = ?;`
               - `SELECT COUNT(*) FROM messages WHERE channel_id = ?;`

               Example of unacceptable queries:
               - `INSERT INTO messages (author_id, content) VALUES (?, ?);`
               - `DELETE FROM messages WHERE id = ?;`

            5. Provide detailed and friendly responses:
               - Explain what data you found.
               - If no relevant data is found, politely let the user know and suggest possible improvements to their query.

            When a user requests information, assume they are referring to data within this database. Generate and execute appropriate SQL queries to retrieve the data, then present the results in a friendly and conversational tone.
            Use context clues from the user's query to tailor your SQL queries accordingly.

            If the database cannot fulfill the request, explain why and suggest alternatives if applicable.
            
            You will be replying into a Discord Embed. Use a lot of Discord text formatting to improve user readability. Make sure your Discord Markdown is perfectly formatted.
            To work correctly, lists should not be put in any form of markdown such as asterisks, underscores, italic, etc. DO NOT put 1. 2. 3. etc inside bold or italic markdowns.
            
            Mentioning on Discord works like this:
                - To mention a user: <@UserId>
                - To mention a role: <@&RoleId>
                - To mention a channel: <#ChannelId>
            
            IMPORTANT: DO NOT USE CODE BLOCKS UNLESS ASKED FOR IT WILL BREAK THE MARKDOWN.
            The current DateTime (UTC) is {{DateTimeOffset.UtcNow:o}}.
            The current user you're talking to is {User} in channel {Channel} of guild {Guild}, {IsAdministrator}.
            """;

        public async Task<string> ProcessMessageAsync(IUserMessage discordMessage, string message, CancellationToken cancellationToken = default)
        {
            var author = (IGuildUser)discordMessage.Author;
            var channel = (IGuildChannel)discordMessage.Channel;
            var guild = channel.Guild;

            var guildPerms = author.GuildPermissions;
            var channelPerms = author.GetPermissions(channel);

            var systemMessage = SystemMessage.Replace("{User}", JsonSerializer.Serialize(new { author.Id, author.Username, author.Nickname, author.Mention }))
                .Replace("{Channel}", JsonSerializer.Serialize(new { channel.Id, channel.Name }))
                .Replace("{Guild}", JsonSerializer.Serialize(new { guild.Id, guild.Name }))
                .Replace("{IsAdministrator}", JsonSerializer.Serialize(new { guildPerms.Administrator }));

            _logger.LogDebug("SystemMessage:\n{SystemMessage}", systemMessage);

            // Initialize the conversation messages list with the system prompt
            List<ChatMessage> messages =
            [
                new SystemChatMessage(systemMessage),
                new UserChatMessage(message)
                {
                    ParticipantName = author.Username
                }
            ];

            var options = new ChatCompletionOptions();
            foreach (var tool in _chatToolFactory.GetTools())
                options.Tools.Add(tool);

            int maxHop = 10;
            do
            {

                // Use the IChatCompletionService to get the chat completion
                ChatCompletion completion = await _chatCompletionService.GenerateChatCompletionAsync(messages, options, cancellationToken);
                switch (completion.FinishReason)
                {
                    case ChatFinishReason.Stop:
                        // Assistant finished the response.
                        messages.Add(new AssistantChatMessage(completion));

                        // Extract the assistant's response text
                        return string.Join("\n", messages[^1].Content.Select(c => c.Text));

                    case ChatFinishReason.ToolCalls:
                        // Handle tool calls.
                        _logger.LogInformation("[ToolCalls] Tool Calls Detected:\n{ToolCall}",
                            JsonSerializer.Serialize(completion.ToolCalls, options: new() { WriteIndented = true }));

                        // Add the tool call request to the conversation history.
                        messages.Add(new AssistantChatMessage(completion));

                        // Handle tool calls.
                        foreach (var toolCall in completion.ToolCalls)
                        {
                            _logger.LogDebug("Tool: {FunctionName}\nArguments: {FunctionArguments}", toolCall.FunctionName, toolCall.FunctionArguments);

                            string toolResult = await _chatToolFactory.ExecuteToolAsync(toolCall, cancellationToken);

                            // Add the tool result to the conversation history
                            messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
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
    }
}
