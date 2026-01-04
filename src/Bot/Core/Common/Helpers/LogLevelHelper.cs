using System.Collections.Frozen;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace DiscordBot.Bot.Core.Common.Helpers;

public static class LogLevelHelper
{
    private static readonly FrozenDictionary<LogSeverity, LogLevel> DiscordToMicrosoftLogLevelMap =
        new Dictionary<LogSeverity, LogLevel>
        {
            [LogSeverity.Critical] = LogLevel.Critical,
            [LogSeverity.Error] = LogLevel.Error,
            [LogSeverity.Warning] = LogLevel.Warning,
            [LogSeverity.Info] = LogLevel.Information,
            [LogSeverity.Verbose] = LogLevel.Debug,
            [LogSeverity.Debug] = LogLevel.Trace
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<LogLevel, LogSeverity> MicrosoftToDiscordLogLevelMap =
        new Dictionary<LogLevel, LogSeverity>
        {
            [LogLevel.Critical] = LogSeverity.Critical,
            [LogLevel.Error] = LogSeverity.Error,
            [LogLevel.Warning] = LogSeverity.Warning,
            [LogLevel.Information] = LogSeverity.Info,
            [LogLevel.Debug] = LogSeverity.Verbose,
            [LogLevel.Trace] = LogSeverity.Debug
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<LogEventLevel, LogLevel> SerilogToMicrosoftLogLevelMap =
        new Dictionary<LogEventLevel, LogLevel>
        {
            [LogEventLevel.Verbose] = LogLevel.Trace,
            [LogEventLevel.Debug] = LogLevel.Debug,
            [LogEventLevel.Information] = LogLevel.Information,
            [LogEventLevel.Warning] = LogLevel.Warning,
            [LogEventLevel.Error] = LogLevel.Error,
            [LogEventLevel.Fatal] = LogLevel.Critical
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<LogLevel, LogEventLevel> MicrosoftToSerilogLogLevelMap =
        new Dictionary<LogLevel, LogEventLevel>
        {
            [LogLevel.Trace] = LogEventLevel.Verbose,
            [LogLevel.Debug] = LogEventLevel.Debug,
            [LogLevel.Information] = LogEventLevel.Information,
            [LogLevel.Warning] = LogEventLevel.Warning,
            [LogLevel.Error] = LogEventLevel.Error,
            [LogLevel.Critical] = LogEventLevel.Fatal
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<LogEventLevel, LogSeverity> SerilogToDiscordLogLevelMap =
        new Dictionary<LogEventLevel, LogSeverity>
        {
            [LogEventLevel.Verbose] = LogSeverity.Debug,
            [LogEventLevel.Debug] = LogSeverity.Verbose,
            [LogEventLevel.Information] = LogSeverity.Info,
            [LogEventLevel.Warning] = LogSeverity.Warning,
            [LogEventLevel.Error] = LogSeverity.Error,
            [LogEventLevel.Fatal] = LogSeverity.Critical
        }.ToFrozenDictionary();

    private static readonly FrozenDictionary<LogSeverity, LogEventLevel> DiscordToSerilogLogLevelMap =
        new Dictionary<LogSeverity, LogEventLevel>
        {
            [LogSeverity.Debug] = LogEventLevel.Verbose,
            [LogSeverity.Verbose] = LogEventLevel.Debug,
            [LogSeverity.Info] = LogEventLevel.Information,
            [LogSeverity.Warning] = LogEventLevel.Warning,
            [LogSeverity.Error] = LogEventLevel.Error,
            [LogSeverity.Critical] = LogEventLevel.Fatal
        }.ToFrozenDictionary();

    public static LogLevel ConvertToMicrosoft(this LogSeverity logSeverity) =>
        DiscordToMicrosoftLogLevelMap.GetValueOrDefault(logSeverity, LogLevel.None);

    public static LogLevel ConvertToMicrosoft(this LogEventLevel logEventLevel) =>
        SerilogToMicrosoftLogLevelMap.GetValueOrDefault(logEventLevel, LogLevel.None);

    public static LogSeverity ConvertToDiscord(this LogLevel logLevel) =>
        MicrosoftToDiscordLogLevelMap.GetValueOrDefault(logLevel, LogSeverity.Info);

    public static LogSeverity ConvertToDiscord(this LogEventLevel logEventLevel) =>
        SerilogToDiscordLogLevelMap.GetValueOrDefault(logEventLevel, LogSeverity.Info);

    public static LogEventLevel ConvertToSerilog(this LogLevel logLevel) =>
        MicrosoftToSerilogLogLevelMap.GetValueOrDefault(logLevel, LogEventLevel.Information);

    public static LogEventLevel ConvertToSerilog(this LogSeverity logSeverity) =>
        DiscordToSerilogLogLevelMap.GetValueOrDefault(logSeverity, LogEventLevel.Information);

    public static LogEventLevel? GetDefaultSerilogLogLevel(IConfiguration configuration)
    {
        var serilogConfig = configuration.GetSection("Serilog:MinimumLevel:Default").Value;
        return Enum.TryParse(serilogConfig, true, out LogEventLevel serilogLevel) ? serilogLevel : null;
    }

    public static LogLevel GetDefaultMicrosoftLogLevel(IConfiguration configuration)
    {
        var logLevelSection = configuration.GetSection("Logging:LogLevel:Default");
        return Enum.TryParse(logLevelSection.Value, out LogLevel logLevel) ? logLevel : LogLevel.None;
    }
}
