using Discord;
using Discord.Interactions;
using MediatR;

namespace DiscordBot.Application.Events;

public record SlashCommandExecutedNotification(SlashCommandInfo Info, IInteractionContext Context, IResult Result) : INotification;

public class SlashCommandExecutedNotificationHandler : INotificationHandler<SlashCommandExecutedNotification>
{
    public Task Handle(SlashCommandExecutedNotification notification, CancellationToken cancellationToken)
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
