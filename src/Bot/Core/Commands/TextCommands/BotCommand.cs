using Discord;
using DiscordBot.Bot.Core.Configurations;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Bot.Core.Commands.TextCommands;

public record BotCommand(IUserMessage Message, string Action, IEnumerable<string>? Args = null);

public sealed class BotCommandHandler(
    ILogger<BotCommandHandler> logger,
    IDiscordClient client,
    IBotConfig botConfig)
{
    public async Task<IUserMessage> Handle(BotCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var message = command.Message;
        var channel = message.Channel;

        var action = command.Action;
        var args = command.Args;

        using var typingState = channel.EnterTypingState();

        try
        {
            return action switch
            {
                "g" or "guild" => await GuildActions(message, args, cancellationToken),
                _ => await HandleUnknownAction(message, action, cancellationToken)
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the message command {Action}.", action);
            return await message.ReplyAsync("Sorry, I couldn't process your request at the moment.", options: new() { CancelToken = cancellationToken });
        }
    }

    private async Task<IUserMessage> HandleUnknownAction(IUserMessage message, string action, CancellationToken cancellationToken)
    {
        logger.LogWarning("Unknown action: {Action}", action);
        return await message.ReplyAsync($"Unknown action: {action}", options: new() { CancelToken = cancellationToken });
    }

    private bool IsAuthorizedAdmin(IUserMessage message)
    {
        var adminIds = botConfig.AdminIds;
        if (adminIds is null or { Length: 0 })
        {
            logger.LogWarning("No admin IDs configured. Denying access to privileged command.");
            return false;
        }

        return adminIds.Contains(message.Author.Id);
    }

    private async Task<IUserMessage> GuildActions(IUserMessage message, IEnumerable<string>? args, CancellationToken cancellationToken)
    {
        if (!args?.Any() ?? true)
            return await message.ReplyAsync("No guild sub-action given.",
                options: new() { CancelToken = cancellationToken });

        var subAction = args!.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(subAction))
            return await message.ReplyAsync("Invalid guild sub-action.",
                options: new() { CancelToken = cancellationToken });

        return subAction switch
        {
            "list" => await ListGuildsAsync(message, cancellationToken),
            "leave" => await LeaveGuildAsync(message, args!.Skip(1), cancellationToken),
            _ => await HandleUnknownGuildSubAction(message, subAction, cancellationToken)
        };
    }

    private async Task<IUserMessage> HandleUnknownGuildSubAction(IUserMessage message, string subAction, CancellationToken cancellationToken)
    {
        logger.LogWarning("Unknown action 'Guild' subAction: {Action}", subAction);
        return await message.ReplyAsync($"Unknown action 'Guild' subAction: {subAction}",
            options: new() { CancelToken = cancellationToken });
    }

    private async Task<IUserMessage> ListGuildsAsync(IUserMessage message, CancellationToken cancellationToken)
    {
        if (!IsAuthorizedAdmin(message))
        {
            logger.LogWarning("Unauthorized access attempt to ListGuildsAsync by user {UserId}", message.Author.Id);
            return await message.ReplyAsync("You are not authorized to use this command.",
                options: new() { CancelToken = cancellationToken });
        }

        var guilds = await client.GetGuildsAsync(options: new() { CancelToken = cancellationToken });
        if (guilds?.Count == 0)
            return await message.ReplyAsync("No guilds found.",
                options: new() { CancelToken = cancellationToken });

        var guildList = string.Join("\n", guilds!.Select((g, c) => $"{c + 1}. {g.Name} ({g.Id})"));

        var embed = new EmbedBuilder()
            .WithTitle("Guilds I am on")
            .WithDescription(guildList)
            .WithColor(Color.Blue)
            .Build();

        return await message.ReplyAsync(embed: embed,
            options: new() { CancelToken = cancellationToken });
    }

    private async Task<IUserMessage> LeaveGuildAsync(IUserMessage message, IEnumerable<string>? args, CancellationToken cancellationToken)
    {
        if (!IsAuthorizedAdmin(message))
        {
            logger.LogWarning("Unauthorized access attempt to LeaveGuildAsync by user {UserId}", message.Author.Id);
            return await message.ReplyAsync("You are not authorized to use this command.",
                options: new() { CancelToken = cancellationToken });
        }

        if (!args?.Any() ?? true)
            return await message.ReplyAsync("No guild ID given.",
                options: new() { CancelToken = cancellationToken });

        var guildIdStr = args!.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(guildIdStr) || !ulong.TryParse(guildIdStr, out var guildId))
            return await message.ReplyAsync("Invalid guild ID.",
                options: new() { CancelToken = cancellationToken });

        var guild = await client.GetGuildAsync(guildId, options: new() { CancelToken = cancellationToken });
        if (guild is null)
            return await message.ReplyAsync($"Guild with ID '{guildIdStr}' not found.",
                options: new() { CancelToken = cancellationToken });

        await guild.LeaveAsync(options: new() { CancelToken = cancellationToken });

        return await message.ReplyAsync($"Left the guild: {guild.Name}",
            options: new() { CancelToken = cancellationToken });
    }
}
