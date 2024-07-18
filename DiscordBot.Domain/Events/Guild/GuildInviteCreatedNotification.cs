using Discord;
using MediatR;

namespace DiscordBot.Domain.Events;

public record GuildInviteCreatedNotification(IInvite Invite) : INotification;