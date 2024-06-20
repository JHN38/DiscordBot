using Discord;
using Discord.Interactions;
using MediatR;

namespace DiscordBot.Domain.Notifications.Commands;

public record ContextCommandExecutedNotification(ContextCommandInfo Info, IInteractionContext Context, IResult Result) : INotification;