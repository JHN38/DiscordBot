using Discord.Interactions;
using DiscordBot.Domain.Events;
using MediatR;

namespace DiscordBot.Application.EventHandlers;
public class ComponentCommandExecutedNotificationHandler : INotificationHandler<ComponentCommandExecutedNotification>
{
    public Task Handle(ComponentCommandExecutedNotification notification, CancellationToken cancellationToken)
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
