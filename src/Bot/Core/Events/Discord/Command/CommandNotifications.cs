using Discord;
using Discord.Interactions;

namespace DiscordBot.Bot.Core.Events.Discord.Command;

public record InteractionSlashCommandExecutedNotification(SlashCommandInfo Info, IInteractionContext Context, IResult Result);

public record InteractionContextCommandExecutedNotification(ContextCommandInfo Info, IInteractionContext Context, IResult Result);

public record InteractionComponentCommandExecutedNotification(ComponentCommandInfo Info, IInteractionContext Context, IResult Result);
