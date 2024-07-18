using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Events;

public record ClientInteractionCreatedNotification(SocketInteraction Interaction) : INotification;
