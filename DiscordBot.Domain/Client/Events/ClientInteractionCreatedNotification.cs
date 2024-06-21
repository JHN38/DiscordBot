using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Client.Events;

public record ClientInteractionCreatedNotification(SocketInteraction Interaction) : INotification;
