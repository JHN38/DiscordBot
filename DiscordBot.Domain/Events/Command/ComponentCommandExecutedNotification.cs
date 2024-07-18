using Discord;
using Discord.Interactions;
using MediatR;

namespace DiscordBot.Domain.Events;

public record ComponentCommandExecutedNotification(ComponentCommandInfo Info, IInteractionContext Context, IResult Result) : INotification;