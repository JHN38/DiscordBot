using MediatR;

namespace DiscordBot.Domain.Client.Events;

public record ClientReadyNotification() : INotification;
