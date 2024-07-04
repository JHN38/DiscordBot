using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Application.Common.Configuration;

public class WeatherOptions
{
    public string? ApiKey { get; init; }
    public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(15);
}
