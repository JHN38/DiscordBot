using Discord;
using DiscordBot.Domain.Commands.Events;
using DiscordBot.Domain.Weather.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands.Events;

public class WeatherCommandNotificationHandler(ILogger<SearchCommandNotificationHandler> logger,
                                               IMediator mediator) : INotificationHandler<WeatherCommandNotification>
{
    public async Task Handle(WeatherCommandNotification notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var location = notification.Location;

        var guildChannel = (IGuildChannel)message.Channel;
        var guild = guildChannel.Guild;
        var author = await guild.GetUserAsync(message.Author.Id);
        var user = author.DisplayName ?? author.GlobalName ?? author.Username;

        using (message.Channel.EnterTypingState()) try
            {
                var embed = notification.WeatherRequestType switch
                {
                    "f" or "forecast" => await mediator.Send(new ForecastWeatherRequest(notification.Location), cancellationToken),
                    _ => await mediator.Send(new CurrentWeatherRequest(notification.Location), cancellationToken)
                };

                if (embed is null)
                {
                    logger.LogWarning("Weather request for \"{Location}\" by user {User} yielded no results.", location, user);
                    await message.ReplyAsync($"Weather request for \"{location}\" yielded no results.");

                    return;
                }

                await message.ReplyAsync(embed: embed);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the search request.");
                await message.ReplyAsync("Sorry, I couldn't process your search request at the moment.");
            }
    }
}
