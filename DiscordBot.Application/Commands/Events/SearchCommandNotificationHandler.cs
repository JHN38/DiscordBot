using ChatGptNet;
using ChatGptNet.Extensions;
using Discord;
using DiscordBot.Application.Common.Helpers;
using DiscordBot.Domain.ChatGpt.Commands;
using DiscordBot.Domain.Commands.Events;
using DiscordBot.Domain.WebSearch.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands.Events;

public class SearchCommandNotificationHandler(ILogger<SearchCommandNotificationHandler> logger,
                                               IMediator mediator) : INotificationHandler<SearchCommandNotification>
{
    public async Task Handle(SearchCommandNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var query = notification.Query;

        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id);
        var user = author.DisplayName ?? author.GlobalName ?? author.Username;

        using (message.Channel.EnterTypingState()) try
        {
            var response = await mediator.Send(new WebSearchRequest(query, notification.ResultCount), cancellationToken);

            if (!response.Any())
            {
                logger.LogWarning("Search \"{Query}\" by user {User} yielded no results.", query, user);
                await message.ReplyAsync($"Search \"{query}\" yielded no results.");
                return;
            }

            var replyCount = 0;
            foreach (var item in response.Take(5))
            {
                var embedBuilder = new EmbedBuilder()
                    .WithTitle(item.Title)
                    .WithUrl(item.Link)
                    .WithColor(Color.Blue)
                    .WithDescription(item.Snippet);

                if (item.ThumbnailUrl != null)
                {
                    embedBuilder.WithThumbnailUrl(item.ThumbnailUrl);
                }

                if (replyCount++ == 0)
                    await message.ReplyAsync(embed: embedBuilder.Build());
                else await message.Channel.SendMessageAsync(embed: embedBuilder.Build());
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the search request.");
            await message.ReplyAsync("Sorry, I couldn't process your search request at the moment.");
        }
    }
}
