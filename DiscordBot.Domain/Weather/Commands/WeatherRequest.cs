﻿using DiscordBot.Domain.Weather.Enums;
using DiscordBot.Domain.Weather.Models;
using MediatR;

namespace DiscordBot.Domain.Weather.Commands;

public partial record WeatherRequest(WeatherRequestType RequestType, string Location, string? Units = null) : IRequest<WeatherResponse?>;
