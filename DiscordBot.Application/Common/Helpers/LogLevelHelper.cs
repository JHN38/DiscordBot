using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace DiscordBot.Application.Common.Helpers;

/// <summary>
/// Provides helper methods for converting log levels between different logging frameworks.
/// </summary>
public static class LogLevelHelper
{
    // Mapping between Discord.Net log severity and Microsoft.Extensions.Logging log level
    private static readonly Dictionary<LogSeverity, LogLevel> _discordToMicrosoftLogLevelMap = new()
    {
        { LogSeverity.Critical, LogLevel.Critical },
        { LogSeverity.Error, LogLevel.Error },
        { LogSeverity.Warning, LogLevel.Warning },
        { LogSeverity.Info, LogLevel.Information },
        { LogSeverity.Verbose, LogLevel.Debug },
        { LogSeverity.Debug, LogLevel.Trace }
    };

    // Mapping between Serilog log event level and Microsoft.Extensions.Logging log level
    private static readonly Dictionary<LogEventLevel, LogLevel> _serilogToMicrosoftLogLevelMap = new()
    {
        { LogEventLevel.Verbose, LogLevel.Trace },
        { LogEventLevel.Debug, LogLevel.Debug },
        { LogEventLevel.Information, LogLevel.Information },
        { LogEventLevel.Warning, LogLevel.Warning },
        { LogEventLevel.Error, LogLevel.Error },
        { LogEventLevel.Fatal, LogLevel.Critical }
    };

    // Mapping between Serilog log event level and Discord.Net log severity
    private static readonly Dictionary<LogEventLevel, LogSeverity> _serilogToDiscordLogLevelMap = new()
    {
        { LogEventLevel.Verbose, LogSeverity.Debug },
        { LogEventLevel.Debug, LogSeverity.Verbose },
        { LogEventLevel.Information, LogSeverity.Info },
        { LogEventLevel.Warning, LogSeverity.Warning },
        { LogEventLevel.Error, LogSeverity.Error },
        { LogEventLevel.Fatal, LogSeverity.Critical }
    };

    // Mapping between Discord.Net log severity and Serilog log event level
    private static readonly Dictionary<LogSeverity, LogEventLevel> _discordToSerilogLogLevelMap = new()
    {
        { LogSeverity.Debug, LogEventLevel.Verbose },
        { LogSeverity.Verbose, LogEventLevel.Debug },
        { LogSeverity.Info, LogEventLevel.Information },
        { LogSeverity.Warning, LogEventLevel.Warning },
        { LogSeverity.Error, LogEventLevel.Error },
        { LogSeverity.Critical, LogEventLevel.Fatal }
    };

    /// <summary>
    /// Converts a Discord.Net <see cref="LogSeverity"/> to a Microsoft.Extensions.Logging <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logSeverity">The Discord.Net log severity.</param>
    /// <returns>The corresponding Microsoft.Extensions.Logging log level.</returns>
    public static LogLevel ConvertToMicrosoft(this LogSeverity logSeverity) =>
        _discordToMicrosoftLogLevelMap.FirstOrDefault(map => map.Key == logSeverity).Value;

    /// <summary>
    /// Converts a Serilog <see cref="LogEventLevel"/> to a Microsoft.Extensions.Logging <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logEventLevel">The Serilog log event level.</param>
    /// <returns>The corresponding Microsoft.Extensions.Logging log level.</returns>
    public static LogLevel ConvertToMicrosoft(this LogEventLevel logEventLevel) =>
        _serilogToMicrosoftLogLevelMap.FirstOrDefault(map => map.Key == logEventLevel).Value;

    /// <summary>
    /// Converts a Microsoft.Extensions.Logging <see cref="LogLevel"/> to a Discord.Net <see cref="LogSeverity"/>.
    /// </summary>
    /// <param name="logLevel">The Microsoft.Extensions.Logging log level.</param>
    /// <returns>The corresponding Discord.Net log severity.</returns>
    public static LogSeverity ConvertToDiscord(this LogLevel logLevel) =>
        _discordToMicrosoftLogLevelMap.FirstOrDefault(map => map.Value == logLevel).Key;

    /// <summary>
    /// Converts a Microsoft.Extensions.Logging <see cref="LogLevel"/> to a Serilog <see cref="LogEventLevel"/>.
    /// </summary>
    /// <param name="logLevel">The Microsoft.Extensions.Logging log level.</param>
    /// <returns>The corresponding Serilog log event level.</returns>
    public static LogEventLevel ConvertToSerilog(this LogLevel logLevel) =>
        _serilogToMicrosoftLogLevelMap.FirstOrDefault(map => map.Value == logLevel).Key;

    /// <summary>
    /// Converts a Serilog <see cref="LogEventLevel"/> to a Discord.Net <see cref="LogSeverity"/>.
    /// </summary>
    /// <param name="logEventLevel">The Serilog log event level.</param>
    /// <returns>The corresponding Discord.Net log severity.</returns>
    public static LogSeverity ConvertToDiscord(this LogEventLevel logEventLevel) =>
        _serilogToDiscordLogLevelMap.FirstOrDefault(map => map.Key == logEventLevel).Value;

    /// <summary>
    /// Converts a Discord.Net <see cref="LogSeverity"/> to a Serilog <see cref="LogEventLevel"/>.
    /// </summary>
    /// <param name="logSeverity">The Discord.Net log severity.</param>
    /// <returns>The corresponding Serilog log event level.</returns>
    public static LogEventLevel ConvertToSerilog(this LogSeverity logSeverity) =>
        _discordToSerilogLogLevelMap.FirstOrDefault(map => map.Key == logSeverity).Value;

    /// <summary>
    /// Retrieves the default Serilog log level from the configuration.
    /// </summary>
    /// <param name="configuration">The configuration containing the Serilog settings.</param>
    /// <returns>The default Serilog log level, or null if the configuration value is invalid.</returns>
    public static LogEventLevel? GetDefaultSerilogLogLevel(IConfiguration configuration)
    {
        var serilogConfig = configuration.GetSection("Serilog:MinimumLevel:Default").Value;
        return Enum.TryParse(serilogConfig, true, out LogEventLevel serilogLevel) ? serilogLevel : null;
    }

    /// <summary>
    /// Retrieves the default Microsoft.Extensions.Logging log level from the configuration.
    /// </summary>
    /// <param name="configuration">The configuration containing the logging settings.</param>
    /// <returns>The default Microsoft.Extensions.Logging log level, or <see cref="LogLevel.None"/> if the configuration value is invalid.</returns>
    public static LogLevel GetDefaultMicrosoftLogLevel(IConfiguration configuration)
    {
        var logLevelSection = configuration.GetSection("Logging:LogLevel:Default");
        return Enum.TryParse(logLevelSection.Value, out LogLevel logLevel) ? logLevel : LogLevel.None;
    }
}