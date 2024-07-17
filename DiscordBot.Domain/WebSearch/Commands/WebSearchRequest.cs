using DiscordBot.Domain.WebSearch.Models;
using MediatR;

namespace DiscordBot.Domain.WebSearch.Commands;

public record WebSearchRequest(string Query, int? ResultCount = null, string? CountryRestriction = null, string? LanguageRestriction = null) : IRequest<WebSearchResult>;
