using Discord;
using Discord.Interactions;
using MediatR;

namespace DiscordBot.Domain.Events;

public record ContextCommandExecutedNotification(ContextCommandInfo Info, IInteractionContext Context, IResult Result) : INotification;