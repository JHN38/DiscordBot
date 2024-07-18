using Discord;
using MediatR;

namespace DiscordBot.Domain.Commands;

public record TextCommand(IUserMessage Message, string Command) : IRequest<IUserMessage?>;