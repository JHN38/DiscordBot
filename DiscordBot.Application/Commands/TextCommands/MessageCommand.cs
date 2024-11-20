using System.Globalization;
using System.Linq;
using Discord;
using DiscordBot.Application.Discord.Messages;
using DiscordBot.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands.TextCommands;

public record MessageCommand(IUserMessage Message, string Action, IEnumerable<string>? Args = null) : IRequest<IUserMessage>;

public class MessageCommandHandler(ILogger<MessageCommandHandler> logger, IDiscordClient client,
    IDiscordMessageRepository messageRepository) : IRequestHandler<MessageCommand, IUserMessage>
{
    private IUserMessage? _message = null!;
    private IMessageChannel? _channel = null!;
    private IGuild? _guild = null!;
    private readonly IDiscordClient _client = client;

    public async Task<IUserMessage> Handle(MessageCommand notification, CancellationToken cancellationToken)
    {
        _message = notification.Message;
        _channel = _message.Channel;
        _guild = ((IGuildChannel)_channel).Guild;

        var action = notification.Action;
        var args = notification.Args;

        // Start typing...
        using var typingState = _channel.EnterTypingState();

        try
        {
            switch (action)
            {
                case "g":
                case "get": return await GetMessagesAsync(args, cancellationToken);
                default:
                    logger.LogWarning("Unknown subcommand: {Action}", action);
                    return await _message.ReplyAsync($"Unknown subcommand: {action}", options: new() { CancelToken = cancellationToken });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the message command {Action}.", action);
            return await _message.ReplyAsync("Sorry, I couldn't process your request at the moment.", options: new() { CancelToken = cancellationToken });
        }
    }

    private async Task<IUserMessage> GetMessagesAsync(IEnumerable<string>? args, CancellationToken cancellationToken = default) =>
        // "last 2"
        args?.FirstOrDefault() switch
        {
            "last" => await GetLastMessagesAsync(args.ElementAtOrDefault(1), cancellationToken),
            "first" => await GetFirstMessagesAsync(args.ElementAtOrDefault(1), cancellationToken),
            "find" => await GetMatchingMessagesAsync(args.ElementAtOrDefault(1), cancellationToken),
            _ => await GetLastMessagesAsync(null, cancellationToken) // "last 1"
        };

    private async Task<IUserMessage> GetMatchingMessagesAsync(string? needle, CancellationToken cancellationToken)
    {
        if (needle is null)
            return await _message.ReplyAsync("No search query given.",
                options: new() { CancelToken = cancellationToken });

        var matchingMessages = await messageRepository.GetMatchingMessagesAsync(needle, cancellationToken);
        // Get all the matching messages and convert to Dictionary<ulong, IUser>
        var authorDisplayNames = await ResolveDisplayNamesAcrossGuildsAsync(matchingMessages.Select(m => m.AuthorId));

        var reply = string.Join("\n", matchingMessages.Select(m =>
            $"{m.Timestamp.ToString("M/dd/yyyy h:mm tt", CultureInfo.InvariantCulture)} **{authorDisplayNames[m.AuthorId]}** {m.Content}"));
        return await _message.ReplyAsync(reply,
            options: new() { CancelToken = cancellationToken });
    }

    private async Task<IUserMessage> GetFirstMessagesAsync(string? numberOfMessagesString, CancellationToken cancellationToken)
    {
        if (!int.TryParse(numberOfMessagesString, out var numberOfMessages))
            numberOfMessages = 1;

        var reply = string.Join("\n", await messageRepository.GetFirstMessagesAsync(numberOfMessages, cancellationToken));
        return await _message.ReplyAsync(reply,
            options: new() { CancelToken = cancellationToken });
    }

    private async Task<IUserMessage> GetLastMessagesAsync(string? numberOfMessagesString, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(numberOfMessagesString, out var numberOfMessages))
            numberOfMessages = 1;

        var reply = string.Join("\n", await messageRepository.GetLastMessagesAsync(numberOfMessages, cancellationToken));
        return await _message.ReplyAsync(reply,
            options: new() { CancelToken = cancellationToken });
    }

    // TODO: this should resolve the displayName on the Guild where the message was sent, if possible.
    // or maybe on the guild where the request was made. TBD
    public async Task<Dictionary<ulong, string>> ResolveDisplayNamesAcrossGuildsAsync(IEnumerable<ulong> userIds)
    {
        var displayNames = new Dictionary<ulong, string>();

        // Iterate through each guild the bot is a member of
        var guilds = await _client.GetGuildsAsync();

        foreach (var guild in guilds)
        {
            // Retrieve all users in the current guild
            var guildUsers = await guild.GetUsersAsync();
            var guildUserDict = guildUsers.ToDictionary(user => user.Id, user => user.DisplayName ?? user.GlobalName ?? user.Username);

            // Loop through the provided user IDs and try to fill in display names if they haven't been resolved yet
            foreach (var userId in userIds.Where(userId => !displayNames.ContainsKey(userId)))
            {
                if (guildUserDict.TryGetValue(userId, out var displayName))
                {
                    displayNames[userId] = displayName;
                }
            }

            // If all user IDs are resolved, break out of the loop
            if (displayNames.Count == userIds.Count())
            {
                break;
            }
        }

        return displayNames;
    }
}

