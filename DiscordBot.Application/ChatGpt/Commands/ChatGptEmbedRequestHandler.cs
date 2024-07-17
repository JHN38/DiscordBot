using System.Text.Json;
using System.Text.Json.Serialization;
using ChatGptNet;
using ChatGptNet.Extensions;
using ChatGptNet.Models;
using Discord;
using DiscordBot.Application.Common.Helpers;
using DiscordBot.Domain.ChatGpt.Commands;
using DiscordBot.Domain.Weather.Commands;
using DiscordBot.Domain.WebSearch.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.ChatGpt.Commands;

public class ChatGptEmbedRequestHandler(ILogger<ChatGptEmbedRequestHandler> logger,
                                   IChatGptClient chatGptClient,
                                   IMediator mediator) : IRequestHandler<ChatGptEmbedRequest>
{
    private readonly List<ChatGptFunction> _functions =
    [
        new()
        {
            Name = "GetCurrentWeather",
            Description = "Get the current weather",
            Parameters = JsonDocument.Parse("""                                        
            {
                "type": "object",
                "properties": {
                    "location": {
                        "type": "string",
                        "description": "The City and/or the Zip code"
                    },
                    "units": {
                        "type": "string",
                        "enum": ["standard", "metric", "imperial"],
                        "description": "The temperature unit to use. Infer this from the location."
                    }
                },
                "required": ["location", "units"]
            }
            """)
        },
        new()
        {
            Name = "GetWeatherForecast",
            Description = "5 day weather forecast data with 3-hour step.",
            Parameters = JsonDocument.Parse("""                                        
            {
                "type": "object",
                "properties": {
                    "location": {
                        "type": "string",
                        "description": "The city and/or the zip code"
                    },
                    "units": {
                        "type": "string",
                        "enum": ["standard", "metric", "imperial"],
                        "description": "The temperature unit to use. Infer this from the user's location."
                    },
                    "steps": {
                        "type": "integer",
                        "description": "The number of 3-hour step(s). Infer this from the prompt if possible."
                    }
                },
                "required": ["location", "units", "steps"]
            }
            """)
        },
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
        }
    ];

    public async Task Handle(ChatGptEmbedRequest request, CancellationToken cancellationToken)
    {
        var message = request.Message;
        var channel = (IMessageChannel)message.Channel;
        var guildChannel = (IGuildChannel)channel;
        var guild = guildChannel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id);
        var user = author.DisplayName ?? author.GlobalName ?? author.Username;
        string dateTimeUtc = DateTimeOffset.UtcNow.ToString("o");

        var conversationId = await chatGptClient.SetupAsync($"""
            Keep your answers brief and to the point. Use functions only when your own data can't answer the prompt. The current datetime is {dateTimeUtc}.
            You will be replying into a Discord Embed. Use a lot of Discord text formatting to improve user readability. Make sure your Discord Markdown is perfectly formatted. DO NOT use code blocks unless necessary.
            To work correctly, lists should not be put in any form of markdown such as asterisks, underscores, italic, etc. DO NOT put 1. 2. 3. etc inside bold or italic markdowns.
            """, cancellationToken: cancellationToken);

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
            return;
        }

        var functionCall = response.GetFunctionCall();
        if (functionCall is not null)
        {
            logger.LogInformation("User {User} called function {Function} with arguments {Arguments}.",
                user, functionCall.Name, functionCall.Arguments);

            logger.LogDebug("Tool Request ({FunctionName}):\n\t{FunctionArguments}", functionCall.Name, functionCall.Arguments);

            var functionResponse = functionCall.Name switch
            {
                "GetCurrentWeather" => await GetCurrentWeatherAsync(functionCall.Arguments, cancellationToken),
                "GetWeatherForecast" => await GetWeatherForecastAsync(functionCall.Arguments, cancellationToken),
                "GetWebSearch" => await WebSearchAsync(functionCall.Arguments, cancellationToken),
                _ => throw new NotSupportedException("Function not supported")
            };

            if (response.GetToolCalls()?.FirstOrDefault() is not ChatGptToolCall tool)
                throw new InvalidOperationException("Tool call not found in the response.");

            logger.LogDebug("Tool Response ({FunctionName}):\n\t{FunctionResponse}", functionCall.Name, functionResponse);

            await chatGptClient.AddToolResponseAsync(conversationId, tool, functionResponse, cancellationToken);

            response = await chatGptClient.AskAsync(conversationId, request.Prompt, toolParameters, cancellationToken: cancellationToken);
        }

        if (response.GetContent() is not string content)
            return;

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

        await message.ReplyAsync(embeds: [.. embedBuilders.ConvertAll(embedBuilder => embedBuilder.Build())]);
    }

    private async Task<string> WebSearchAsync(string? arguments, CancellationToken cancellationToken = default)
    {
        if (arguments is null)
            return string.Empty;

        var args = JsonSerializer.Deserialize<GptWebSearchArguments>(arguments) ?? throw new InvalidOperationException("Invalid JSON format for weather arguments.");
        var response = await mediator.Send(new WebSearchRequest(args.Query, 10, args.CountryRestriction, args.LanguageRestriction), cancellationToken);

        if (response.Items.IsEmpty)
            return "No search results found.";

        return JsonSerializer.Serialize(response);
    }

    private async Task<string> GetCurrentWeatherAsync(string? arguments, CancellationToken cancellationToken = default)
    {
        if (arguments is null)
            return string.Empty;

        var args = JsonSerializer.Deserialize<GptWeatherArguments>(arguments) ?? throw new InvalidOperationException("Invalid JSON format for weather arguments.");
        var response = await mediator.Send(new WeatherRequest(Domain.Weather.Enums.WeatherRequestType.Weather, args.Location, args.Units), cancellationToken);

        return JsonSerializer.Serialize(response);
    }

    private async Task<string> GetWeatherForecastAsync(string? arguments, CancellationToken cancellationToken = default)
    {
        if (arguments is null)
            return string.Empty;

        var args = JsonSerializer.Deserialize<GptWeatherArguments>(arguments) ?? throw new InvalidOperationException("Invalid JSON format for weather arguments.");
        var response = await mediator.Send(new WeatherRequest(Domain.Weather.Enums.WeatherRequestType.Forecast, args.Location, args.Units), cancellationToken);

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
