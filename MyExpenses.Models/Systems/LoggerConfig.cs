using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MyExpenses.Models.Systems;

public static class LoggerConfig
{
    private const string Template = "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}";
    private static readonly string DefaultFilename = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";

    /// <summary>
    /// Configures the logger to write log events to the specified destinations.
    /// </summary>
    /// <param name="loggerConfiguration">The logger configuration to which the write options will be applied.</param>
    /// <param name="toConsole">Indicates whether log events should be written to the console.</param>
    /// <param name="toFile">Indicates whether log events should be written to a file.</param>
    /// <param name="basePath">The base directory path where log files will be written. If null, defaults to the current directory.</param>
    public static void SetWriteToOption(this LoggerConfiguration loggerConfiguration,
        bool toConsole = false, bool toFile = false, string? basePath = null)
    {
        if (toConsole)
        {
            loggerConfiguration.WriteTo.Console(outputTemplate: Template, theme: AnsiConsoleTheme.Code,
                applyThemeToRedirectedOutput: true);
        }

        if (toFile)
        {
            var logPath = Path.Join(basePath, DefaultFilename);
            loggerConfiguration.WriteTo.File(logPath, outputTemplate: Template, flushToDiskInterval: TimeSpan.FromSeconds(1), shared:true);
        }
    }

    /// <summary>
    /// Sets the minimum logging level for the specified logger configuration.
    /// </summary>
    /// <param name="loggerConfiguration">The logger configuration to modify.</param>
    /// <param name="level">The desired minimum logging level, or null for the default level.</param>
    public static void SetLoggerConfigurationLevel(this LoggerConfiguration loggerConfiguration, LogEventLevel? level)
    {
        switch (level)
        {
            case LogEventLevel.Information:
                loggerConfiguration.MinimumLevel.Information();
                break;
            case LogEventLevel.Debug:
                loggerConfiguration.MinimumLevel.Debug();
                break;
            case LogEventLevel.Warning:
                loggerConfiguration.MinimumLevel.Warning();
                break;
            case LogEventLevel.Error:
                loggerConfiguration.MinimumLevel.Error();
                break;
            case LogEventLevel.Fatal:
                loggerConfiguration.MinimumLevel.Fatal();
                break;
            case LogEventLevel.Verbose:
            default:
                loggerConfiguration.MinimumLevel.Verbose();
                break;
        }
    }
}