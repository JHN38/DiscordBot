using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record GuildPresenceUpdateNotification(IUser User, IPresence OldPresence, IPresence NewPresence) : INotification;
