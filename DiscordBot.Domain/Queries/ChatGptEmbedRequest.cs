using Discord;
using MediatR;

namespace DiscordBot.Domain.Queries;

public record ChatGptEmbedRequest(IUserMessage Message, string Prompt) : IRequest<IUserMessage>;