using Discord;
using Discord.Interactions;
using MediatR;

namespace DiscordBot.Domain.Events;

public record SlashCommandExecutedNotification(SlashCommandInfo Info, IInteractionContext Context, IResult Result) : INotification;