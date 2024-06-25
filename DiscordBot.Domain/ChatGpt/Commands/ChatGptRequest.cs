using ChatGptNet.Models;
using MediatR;

namespace DiscordBot.Domain.ChatGpt.Commands;

public record ChatGptRequest(string User, string Query) : IRequest<string>;