﻿using Discord.WebSocket;
using DiscordBot.Domain.WebSearch.Models;
using MediatR;

namespace DiscordBot.Domain.WebSearch.Commands;

public record WebSearchRequest(string Query, int ResultCount = 1) : IRequest<IEnumerable<WebSearchResponse>>;
