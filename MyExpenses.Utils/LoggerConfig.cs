using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace MyExpenses.Utils;

/// <summary>
/// Provides methods for configuring the logger.
/// </summary>
public static class LoggerConfig
{
    private static string LogDirectoryPath { get; } = Directory.CreateDirectory("log").FullName;

    /// <summary>
    /// Creates a logger configuration for the application.
    /// </summary>
    /// <returns>A Logger object representing the configured logger.</returns>
    public static Logger CreateConfig()
    {
        const string template = "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}";
        var logName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";

        var logPath = Path.Combine(LogDirectoryPath, logName);
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: template, theme: AnsiConsoleTheme.Code, applyThemeToRedirectedOutput: true)
            .WriteTo.File(logPath, outputTemplate: template, flushToDiskInterval: TimeSpan.FromSeconds(1))
            .CreateLogger();

        return loggerConfiguration;
    }
}