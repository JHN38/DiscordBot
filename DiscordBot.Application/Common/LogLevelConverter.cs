using Discord;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Application.Common;
public sealed class LogLevelConverter
{
    private static readonly Dictionary<LogSeverity, LogLevel> _map = new()
    {
        { LogSeverity.Critical, LogLevel.Critical },
        { LogSeverity.Error, LogLevel.Error },
        { LogSeverity.Warning, LogLevel.Warning },
        { LogSeverity.Info, LogLevel.Information },
        { LogSeverity.Verbose, LogLevel.Debug },
        { LogSeverity.Debug, LogLevel.Trace }
    };

    public static LogLevel Convert(LogSeverity logSeverity) => _map.FirstOrDefault(map => map.Key == logSeverity).Value;
    public static LogSeverity Convert(LogLevel logLevel) => _map.FirstOrDefault(map => map.Value == logLevel).Key;
}
