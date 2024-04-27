using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace MyExpenses.Utils;

public static class LoggerConfig
{
    public static Logger CreateConfig()
    {
        const string template = "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}";
        var logName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
        var directory = Directory.CreateDirectory("log").FullName;

        var logPath = Path.Combine(directory, logName);
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: template, theme: AnsiConsoleTheme.Code, applyThemeToRedirectedOutput: true)
            .WriteTo.File(logPath, outputTemplate: template, flushToDiskInterval: TimeSpan.FromSeconds(1))
            .CreateLogger();

        return loggerConfiguration;
    }
}