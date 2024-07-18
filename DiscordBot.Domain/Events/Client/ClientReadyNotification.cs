using MediatR;

namespace DiscordBot.Domain.Events;

public record ClientReadyNotification() : INotification;
