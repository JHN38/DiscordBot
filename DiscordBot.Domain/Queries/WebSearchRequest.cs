using DiscordBot.Domain.Entities;
using MediatR;

namespace DiscordBot.Domain.Queries;

public record WebSearchRequest(string Query, int? ResultCount = null, string? CountryRestriction = null, string? LanguageRestriction = null) : IRequest<WebSearchResponse>;
