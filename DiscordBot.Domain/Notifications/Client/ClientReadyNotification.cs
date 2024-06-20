using MediatR;

namespace DiscordBot.Domain.Notifications.Client;

public record ClientReadyNotification() : INotification;
