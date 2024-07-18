using Discord;
using MediatR;

namespace DiscordBot.Domain.Commands;

public record MentionCommand(IUserMessage Message) : IRequest<IUserMessage?>;