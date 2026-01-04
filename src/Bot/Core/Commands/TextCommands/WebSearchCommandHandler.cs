using Discord;
using DiscordBot.Contracts.WebSearch;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace DiscordBot.Bot.Core.Commands.TextCommands;

public record WebSearchCommand(IUserMessage Message, string Query, int ResultCount = 1, string? Country = null);

public sealed class WebSearchCommandHandler(ILogger<WebSearchCommandHandler> logger, IMessageBus bus)
{
    public async Task<IUserMessage> Handle(WebSearchCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var message = command.Message;
        var query = command.Query;
        var channel = message.Channel;

        if (channel is not IGuildChannel guildChannel)
        {
            return await message.ReplyAsync("This command can only be used in a server channel.");
        }

        var guild = guildChannel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id);
        var user = author.DisplayName ?? author.GlobalName ?? author.Username;

        using var typingState = channel.EnterTypingState();

        if (string.IsNullOrWhiteSpace(query))
            return await message.ReplyAsync("No search query given.");

        try
        {
            var contractResponse = await bus.InvokeAsync<WebSearchResponse>(new SearchWebQuery(query), cancellationToken);

            if (contractResponse is null || !contractResponse.Items.Any())
            {
                logger.LogWarning("Search \"{Query}\" by user {User} yielded no results.", query, user);
                return await message.ReplyAsync($"Search \"{query}\" yielded no results.");
            }

            var embedBuilders = BuildResponseEmbeds(contractResponse);
            return await message.ReplyAsync(embeds: [.. embedBuilders.Select(embedBuilder => embedBuilder.Build())]);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the search request.");
            return await message.ReplyAsync("Sorry, I couldn't process your search request at the moment.");
        }
    }

    private static IEnumerable<EmbedBuilder> BuildResponseEmbeds(WebSearchResponse response)
    {
        foreach (var item in response.Items)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(item.Title)
                .WithUrl(item.Link.ToString())
                .WithColor(Color.Blue)
                .WithDescription(item.Snippet);

            if (item.Thumbnails.Any() && item.Thumbnails.First() is { } thumbnail)
            {
                embedBuilder.WithThumbnailUrl(thumbnail.Src.ToString());
            }

            yield return embedBuilder;
        }
    }
}
