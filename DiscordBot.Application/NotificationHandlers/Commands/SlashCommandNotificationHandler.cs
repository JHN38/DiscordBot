﻿using Discord.Interactions;
using DiscordBot.Domain.Notifications.Commands;
using MediatR;

namespace DiscordBot.Application.NotificationHandlers.Commands;
public class SlashCommandNotificationHandler : INotificationHandler<SlashCommandExecutedNotification>
{
    public Task Handle(SlashCommandExecutedNotification notification, CancellationToken cancellationToken)
    {
        if (!notification.Result.IsSuccess)
        {
            switch (notification.Result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    // implement
                    break;
                case InteractionCommandError.Exception:
                    // implement
                    break;
                case InteractionCommandError.Unsuccessful:
                    // implement
                    break;
                default:
                    break;
            }
        }

        return Task.CompletedTask;
    }
}
