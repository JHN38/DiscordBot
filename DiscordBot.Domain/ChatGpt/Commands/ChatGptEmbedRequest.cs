using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.ChatGpt.Commands;

public record ChatGptEmbedRequest(SocketUserMessage Message, string Prompt) : IRequest;