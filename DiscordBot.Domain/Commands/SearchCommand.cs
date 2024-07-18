using Discord;
using MediatR;

namespace DiscordBot.Domain.Commands;

public record SearchCommand(IUserMessage Message, string Query, int ResultCount = 1, string? Country = null) : IRequest<IUserMessage>;