using Discord;
using DiscordBot.Application.Queries;
using DiscordBot.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands.TextCommands;

public record WebSearchCommand(IUserMessage Message, string Query, int ResultCount = 1, string? Country = null) : IRequest<IUserMessage>;

public class WebSearchCommandHandler(ILogger<WebSearchCommandHandler> logger,
                                               IMediator mediator) : IRequestHandler<WebSearchCommand, IUserMessage>
{
    public async Task<IUserMessage> Handle(WebSearchCommand notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var query = notification.Query;
        var channel = message.Channel;
        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id);
        var user = author.DisplayName ?? author.GlobalName ?? author.Username;

        using var typingState = channel.EnterTypingState();

        if (string.IsNullOrWhiteSpace(query))
            return await message.ReplyAsync("No search query given.");

        try
        {
            var response = await mediator.Send(new WebSearchRequest(query, notification.ResultCount, notification.Country), cancellationToken);

            if (!response.Items.Any())
            {
                logger.LogWarning("Search \"{Query}\" by user {User} yielded no results.", query, user);
                return await message.ReplyAsync($"Search \"{query}\" yielded no results.");
            }

            var embedBuilders = BuildResponseEmbeds(response);
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
                .WithUrl(item.Link)
                .WithColor(Color.Blue)
                .WithDescription(item.Snippet);

            if (item.Thumbnails.Any() && item.Thumbnails.First(t => t.Src != null) is WebSearchImage thumbnail)
            {
                embedBuilder.WithThumbnailUrl(thumbnail.Src);
            }

            yield return embedBuilder;
        }
    }
}
