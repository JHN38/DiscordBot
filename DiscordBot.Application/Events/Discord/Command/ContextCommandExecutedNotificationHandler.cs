using Discord;
using Discord.Interactions;
using MediatR;

namespace DiscordBot.Application.Events;

public record ContextCommandExecutedNotification(ContextCommandInfo Info, IInteractionContext Context, IResult Result) : INotification;

public class ContextCommandExecutedNotificationHandler : INotificationHandler<ContextCommandExecutedNotification>
{
    public Task Handle(ContextCommandExecutedNotification notification, CancellationToken cancellationToken)
    {
        if (!notification.Result.IsSuccess)
        {
            switch (notification.Result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                case InteractionCommandError.UnknownCommand:
                case InteractionCommandError.BadArgs:
                case InteractionCommandError.Exception:
                case InteractionCommandError.Unsuccessful:
                default:
                    break;
            }
        }

        return Task.CompletedTask;
    }
}
