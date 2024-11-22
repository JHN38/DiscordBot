using System;
using System.Globalization;
using Discord;
using DiscordBot.Application.Discord.Messages;
using DiscordBot.Application.Interfaces;
using DiscordBot.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Commands.TextCommands;

public record MessageCommand(IUserMessage Message, string Action, IEnumerable<string>? Args = null) : IRequest<IUserMessage>;

public class MessageCommandHandler(ILogger<MessageCommandHandler> logger,
    IDiscordUserDisplayNameResolver displayNameResolver,
    IDiscordMessageRepository messageRepository) : IRequestHandler<MessageCommand, IUserMessage>
{
    private IUserMessage? _message = null!;
    private IGuild? _guild = null!;
    private readonly IDiscordUserDisplayNameResolver _displayNameResolver = displayNameResolver;

    public async Task<IUserMessage> Handle(MessageCommand notification, CancellationToken cancellationToken)
    {
        _message = notification.Message;
        var channel = _message.Channel;
        _guild = ((IGuildChannel)channel).Guild;

        var action = notification.Action;
        var args = notification.Args;

        // Start typing...
        using var typingState = channel.EnterTypingState();

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

        if (!matchingMessages?.Any() ?? true)
        {
            return await _message.ReplyAsync("No matching message(s) found.",
                options: new() { CancelToken = cancellationToken });
        }

        var guildUserPairs = matchingMessages!
                .Select(m => new DiscordGuildUserPair(m.GuildId, m.AuthorId));

        var authorDisplayNames = await _displayNameResolver.ResolveDisplayNamesAsync(guildUserPairs, _guild!.Id, cancellationToken);

        var formattedMessages = matchingMessages!
            .Join(guildUserPairs,
                m => m.AuthorId,
                p => p.UserId,
                (m, p) => 
                    $"{m.Timestamp.ToString("M/dd/yyyy h:mm tt", CultureInfo.InvariantCulture)} **{authorDisplayNames[p]}** {m.Content}");

        return await _message.ReplyAsync(string.Join("\n", formattedMessages),
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
}


