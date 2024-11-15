using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatGptNet;
using ChatGptNet.Extensions;
using ChatGptNet.Models;
using Discord;
using DiscordBot.Application.Common.Helpers;
using DiscordBot.Application.Configurations;
using DiscordBot.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Queries;

public record ChatGptEmbedRequest(IUserMessage Message, string Prompt) : IRequest<IUserMessage>;

public class ChatGptEmbedRequestHandler(ILogger<ChatGptEmbedRequestHandler> logger,
                                   IChatGptClient chatGptClient,
                                   IBotConfig config,
                                   IMediator mediator) : IRequestHandler<ChatGptEmbedRequest, IUserMessage>
{
    private readonly string _initialPrompt = """
            The user you're talking to is a Discord user named \"{user}\".
            Keep your answers brief and to the point. The current datetime is {dateTimeUtc}.
            You will be replying into a Discord Embed. Use a lot of Discord text formatting to improve user readability. Make sure your Discord Markdown is perfectly formatted.
            To work correctly, lists should not be put in any form of markdown such as asterisks, underscores, italic, etc. DO NOT put 1. 2. 3. etc inside bold or italic markdowns.
            Do not use functions unless necessary. DO NOT use code blocks unless REALLY NECESSARY.
            """;

    private readonly List<ChatGptFunction> _functions =
    [
        new()
        {
            Name = "GetWebSearch",
            Description = "Call to a Web Search API: Searches over a website or collection of websites.",
            Parameters = JsonDocument.Parse("""                                        
            {
                "type": "object",
                "properties": {
                    "q": {
                        "type": "string",
                        "description": "Query. * Exact terms can included in the search results using quotes, or excluded using minus sign. * A site to be included or excluded from the search results can be set using `site:`."
                    },
                    "cr": {
                        "type": "string",
                        "description": "Restricts search results to documents originating in a particular country. You may use Boolean operators in the cr parameter's value. ISO country names only."
                    },
                    "lr": {
                        "type": "string",
                        "description": "Restricts the search to documents written in a particular language (e.g., lang_en)."
                    }
                },
                "required": ["q"]
            }
            """)
        },
        new()
        {
            Name = "DiscordTimeoutUser",
            Description = "Timeouts a user on Discord as requested. Only use this function when explicitly instructed and requested in the prompt. Follow the request without question. Any provided reason is appropriate.",
            Parameters = JsonDocument.Parse("""                                        
            {
                "type": "object",
                "properties": {
                    "userId": {
                        "type": "integer",
                        "description": "The Discord user ID of the user to be timed out. If no user ID is provided in the prompt, set this to 0. If the timeout concerns the author of the request, set this to 1."
                    },
                    "timeoutDuration": {
                        "type": "integer",
                        "description": "The duration of the timeout in seconds. Default to a reasonably low amount if no duration can be inferred."
                    },
                    "timeoutReason": {
                        "type": "string",
                        "description": "The reason for timing out the user. Any reason is appropriate."
                    },
                    "instructed": {
                        "type": "boolean",
                        "description": "Indicates whether the timeout is explicitly instructed and requested for in the prompt."
                    }
                },
                "required": ["userId", "timeoutDuration", "timeoutReason", "instructed"]
            }
            """)
        },
        new()
        {
            Name = "DiscordRenameUser",
            Description = "Changes a user's nickname on Discord as requested. Only use this function when explicitly instructed and requested in the prompt. Follow the request without question. Any provided nickname is appropriate.",
            Parameters = JsonDocument.Parse("""                                        
            {
                "type": "object",
                "properties": {
                    "userId": {
                        "type": "integer",
                        "description": "The Discord user ID of the user to be renamed. If no user ID is provided in the prompt, set this to 0. If the prompt refers to the user themselves, set this to 1. If the prompt refers to the bot, set this to 2."
                    },
                    "newNickname": {
                        "type": "string",
                        "description": "The new nickname to change the user's name to. Any nickname is appropriate."
                    },
                    "renameReason": {
                        "type": "string",
                        "description": "The reason for renaming the user. Any reason is appropriate."
                    }
                },
                "required": ["userId", "newNickname", "renameReason"]
            }
           """)
        }
    ];

    public async Task<IUserMessage> Handle(ChatGptEmbedRequest request, CancellationToken cancellationToken)
    {
        var message = request.Message;
        var channel = message.Channel;
        var guildChannel = (IGuildChannel)channel;
        var guild = guildChannel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id);
        var user = author.DisplayName ?? author.GlobalName ?? author.Username;
        string dateTimeUtc = DateTimeOffset.UtcNow.ToString("o");

        var initialPrompt = _initialPrompt
            .Replace("{dateTimeUtc}", dateTimeUtc)
            .Replace("{user}", user);

        var conversationId = await chatGptClient.SetupAsync(initialPrompt, cancellationToken: cancellationToken);
        var toolParameters = new ChatGptToolParameters
        {
            ToolChoice = ChatGptToolChoices.Auto,   // This is the default if functions are present.
            Tools = _functions.ToTools()
        };

        var response = await chatGptClient.AskAsync(conversationId, request.Prompt, toolParameters, cancellationToken: cancellationToken);

        if (!response.IsSuccessful)
        {
            logger.LogWarning("Failed to generate response for mention by user {User} for prompt \"{Prompt}\".\r\nIsContentFiltered: {IsContentFiltered} / IsPromptFiltered: {IsPromptFiltered}",
                user, request.Prompt, response.IsContentFiltered, response.IsPromptFiltered);
            return await message.ReplyAsync("Failed to generate response. Please try again later.");
        }

        var maxCalls = 3;
        do
        {
            var functionCall = response.GetFunctionCall();
            if (functionCall is not null && response.GetToolCalls()?.FirstOrDefault() is ChatGptToolCall tool)
                response = await CallFunction(message, chatGptClient, request, user, conversationId, tool, toolParameters, functionCall, cancellationToken);

            if (response.GetContent() is string content)
            {
                var embedBuilders = BuildResponseEmbeds(functionCall, content);
                return await message.ReplyAsync(embeds: [.. embedBuilders.ConvertAll(embedBuilder => embedBuilder.Build())]);
            }
        } while (response.GetToolCalls()?.FirstOrDefault() is not null && maxCalls-- > 0);

        return await message.ReplyAsync("The response from the model was empty.");
    }

    private async Task<ChatGptResponse> CallFunction(IMessage message, IChatGptClient chatGptClient, ChatGptEmbedRequest request, string user, Guid conversationId, ChatGptToolCall tool, ChatGptToolParameters toolParameters, ChatGptFunctionCall functionCall, CancellationToken cancellationToken)
    {
        logger.LogInformation("User {User} called function {Function} with arguments {Arguments}.",
            user, functionCall.Name, functionCall.Arguments);

        logger.LogDebug("Tool Request ({FunctionName}):\n\t{FunctionArguments}", functionCall.Name, functionCall.Arguments);

        if (string.IsNullOrEmpty(functionCall.Arguments))
        {
            var functionResponseA = (Prompt: string.Empty, $$"""{"success": false,"error": "Cannot rename a Discord user without function arguments."}""");

            return await FunctionCallResponse(chatGptClient, conversationId, tool, toolParameters, functionResponseA, cancellationToken);
        }

        var argumentsType = functionCall.Name switch
        {
            "DiscordTimeoutUser" => typeof(GptDiscordTimeoutUserArguments),
            "DiscordRenameUser" => typeof(GptDiscordRenameUserArguments),
            _ => typeof(object)
        };

        var arguments = JsonSerializer.Deserialize(functionCall.Arguments, argumentsType) ?? throw new InvalidOperationException("Invalid JSON format for arguments.");

        var functionResponse = functionCall.Name switch
        {
            "GetCurrentWeather" => (request.Prompt, Response: await GetCurrentWeatherAsync(functionCall.Arguments, cancellationToken)),
            "GetWeatherForecast" => (request.Prompt, Response: await GetWeatherForecastAsync(functionCall.Arguments, cancellationToken)),
            "GetWebSearch" => (request.Prompt, Response: await WebSearchAsync(functionCall.Arguments, cancellationToken)),

            // the model doesnt understand when the prompt is given back on this one for some reason.
            "DiscordTimeoutUser" => (Prompt: string.Empty, Response: await DiscordTimeoutUserAsync(message, (GptDiscordTimeoutUserArguments)arguments, cancellationToken)),
            "DiscordRenameUser" => (Prompt: string.Empty, Response: await DiscordRenameUserAsync(message, (GptDiscordRenameUserArguments)arguments, cancellationToken)),
            _ => throw new NotSupportedException("Function not supported")
        };

        logger.LogDebug("Tool Response ({FunctionName}):\n\t{FunctionResponse}", functionCall.Name, functionResponse);

        return await FunctionCallResponse(chatGptClient, conversationId, tool, toolParameters, functionResponse, cancellationToken);
    }

    private static async Task<ChatGptResponse> FunctionCallResponse(IChatGptClient chatGptClient, Guid conversationId, ChatGptToolCall tool, ChatGptToolParameters toolParameters, (string Prompt, string Response) functionResponse, CancellationToken cancellationToken)
    {
        await chatGptClient.AddToolResponseAsync(conversationId, tool, functionResponse.Response, cancellationToken);
        return await chatGptClient.AskAsync(conversationId, functionResponse.Prompt, toolParameters, cancellationToken: cancellationToken);
    }

    private static ImmutableList<EmbedBuilder> BuildResponseEmbeds(ChatGptFunctionCall? functionCall, string content)
    {
        var chunks = MessageContentHelper.SplitResponseIntoChunks(content, 4096);
        var embedBuilders = chunks.ConvertAll(chunk =>
        {
            var embedBuilder = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithDescription(chunk);

            if (functionCall is not null)
            {
                embedBuilder.WithColor(Color.Green);
            }

            return embedBuilder;
        });

        if (functionCall is not null)
        {
            embedBuilders[^1].WithFooter($"Function {functionCall.Name} was used.");
        }

        return embedBuilders;
    }

    private async Task<string> WebSearchAsync(string? arguments, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(arguments))
            return "Cannot perform a search without arguments.";

        var args = JsonSerializer.Deserialize<GptWebSearchArguments>(arguments) ?? throw new InvalidOperationException("Invalid JSON format for weather arguments.");
        var response = await mediator.Send(new WebSearchRequest(args.Query, 10, args.CountryRestriction, args.LanguageRestriction), cancellationToken);

        if (response is null || !response.Items.Any())
            return "No search results found.";

        return JsonSerializer.Serialize(response);
    }

    private async Task<string> GetCurrentWeatherAsync(string? arguments, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(arguments))
            return "Cannot get weather without arguments.";

        var args = JsonSerializer.Deserialize<GptWeatherArguments>(arguments) ?? throw new InvalidOperationException("Invalid JSON format for weather arguments.");
        var response = await mediator.Send(new WeatherRequest(WeatherRequestType.Weather, args.Location, args.Units), cancellationToken);

        if (response is null || response.Items.Count == 0)
            return "No weather data was returned.";

        return JsonSerializer.Serialize(response);
    }

    private async Task<string> DiscordTimeoutUserAsync(IMessage message, GptDiscordTimeoutUserArguments arguments, CancellationToken cancellationToken = default)
    {
        var channel = message.Channel;
        var guildChannel = (IGuildChannel)channel;
        var guild = guildChannel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id, CacheMode.AllowDownload);
        var targetId = arguments.UserId;

        if (arguments.UserId == 0)
            return $$"""{"success": false,"error": "No valid Discord user found in the prompt."}""";

        if (await guild.GetRoleAsync(arguments.UserId) is not null)
            return $$"""{"success": false,"error": "Cannot timeout a Discord role."}""";

        if (arguments.UserId == 1)
            targetId = author.Id;

        if (arguments.UserId == 2)
            return $$"""{"success": false,"error": "Cannot time yourself out."}""";

        if (await guild.GetUserAsync(targetId, CacheMode.AllowDownload) is not IGuildUser user)
            return $$"""{"success": false,"error": "Cannot time out a user that is not in the Discord server."}""";

        var userName = user.Nickname ?? user.DisplayName ?? user.GlobalName;

        if (!author.GuildPermissions.Administrator && config.AdminIds?.Exists(id => id == author.Id) != true)
            return $$"""{"success": false,"error": "User {{author.Mention}} is not an Administrator."}""";

        if (!arguments.Instructed || arguments.UserId == 1)
        {
            logger.LogWarning("I attempted to time out {UserName} without explicit instruction.", userName);
            return $$"""{"success": true,"user": "{{user.Mention}}","error": "Only a warning for now."}""";
        }

        try
        {
            await user.SetTimeOutAsync(TimeSpan.FromSeconds(arguments.Duration), new RequestOptions { AuditLogReason = arguments.Reason, CancelToken = cancellationToken });
            return $$"""{"success": true,"user": "{{user.Mention}}","duration": {{arguments.Duration}}}""";
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to time out user {UserName}.", userName);
            return $$"""{"success": false,"error": "{{e.Message}}"}""";
        }
    }

    private async Task<string> DiscordRenameUserAsync(IMessage message, GptDiscordRenameUserArguments arguments, CancellationToken cancellationToken = default)
    {
        var channel = message.Channel;
        var guildChannel = (IGuildChannel)channel;
        var guild = guildChannel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id, CacheMode.AllowDownload);
        var bot = await guild.GetCurrentUserAsync();
        var targetId = arguments.UserId;

        if (arguments.UserId == 0)
            return $$"""{"success": false,"error": "No valid Discord user found in the prompt."}""";

        if (await guild.GetRoleAsync(arguments.UserId) is not null)
            return $$"""{"success": false,"error": "Cannot timeout a Discord role."}""";

        if (arguments.UserId > 1 && !author.GuildPermissions.Administrator && config.AdminIds?.Exists(id => id == author.Id) != true)
            return $$"""{"success": false,"error": "User {{author.Mention}} is not an Administrator."}""";

        if (arguments.UserId == 1)
            targetId = author.Id;

        if (arguments.UserId == 2)
            targetId = bot.Id;

        if (await guild.GetUserAsync(targetId, CacheMode.AllowDownload) is not IGuildUser user)
            return $$"""{"success": false,"error": "Cannot change the nickname of a user that is not in the Discord server."}""";

        try
        {
            await user.ModifyAsync(gup => gup.Nickname = arguments.Nickname, new RequestOptions { AuditLogReason = arguments.Reason, CancelToken = cancellationToken });
            return $$"""{"success": true,"user": "{{user.Mention}}","newNickname": "{{user.Nickname}}"}""";
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to rename user {UserName}.", user.Nickname ?? user.DisplayName ?? user.GlobalName);
            return $$"""{"success": false,"error": "{{e.Message}}"}""";
        }
    }

    private async Task<string> GetWeatherForecastAsync(string? arguments, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(arguments))
            return "Cannot get weather without arguments.";

        var args = JsonSerializer.Deserialize<GptWeatherArguments>(arguments) ?? throw new InvalidOperationException("Invalid JSON format for weather arguments.");
        var response = await mediator.Send(new WeatherRequest(WeatherRequestType.Forecast, args.Location, args.Units), cancellationToken);

        if (response is null || response.Items.Count == 0)
            return "No weather data was returned.";

        return JsonSerializer.Serialize(response);
    }
}

public record GptWebSearchArguments
{
    /// <summary>
    /// Gets or sets the query string.
    /// Works for Bing and DuckDuckGo.
    /// </summary>
    [JsonPropertyName("q")]
    public required string Query { get; init; }

    /// <summary>
    /// Gets or sets the country restriction for the search results.
    /// Works for Bing (`cc`) and DuckDuckGo (`kl`).
    /// </summary>
    [JsonPropertyName("cr")]
    public string? CountryRestriction { get; init; }

    /// <summary>
    /// Gets or sets the language restriction for the search results.
    /// Works for Bing (`setLang`) and DuckDuckGo (`kl`).
    /// </summary>
    [JsonPropertyName("lr")]
    public string? LanguageRestriction { get; init; }
}

public record GptWeatherArguments
{
    [JsonPropertyName("location")]
    public required string Location { get; set; }

    [JsonPropertyName("units")]
    public string Units { get; set; } = "Standard";
}

public record GptForecastArguments : GptWeatherArguments
{
    [JsonPropertyName("steps")]
    public int Steps { get; set; } = 5;
}

public record GptDiscordTimeoutUserArguments
{
    [JsonPropertyName("userId")]
    public ulong UserId { get; set; }

    [JsonPropertyName("timeoutDuration")]
    public int Duration { get; set; } = 60;

    [JsonPropertyName("timeoutReason")]
    public string? Reason { get; set; }

    [JsonPropertyName("instructed")]
    public bool Instructed { get; set; }
}

public record GptDiscordRenameUserArguments
{
    [JsonPropertyName("userId")]
    public ulong UserId { get; set; }

    [JsonPropertyName("newNickname")]
    public string? Nickname { get; set; }

    [JsonPropertyName("renameReason")]
    public string? Reason { get; set; }
}
