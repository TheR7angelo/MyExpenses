using MyExpenses.Models.Systems;
using MyExpenses.Sql.Context;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MyExpenses.Utils;

/// <summary>
/// Provides methods for configuring the logger.
/// </summary>
public static class LoggerConfig
{
    private static string LogDirectoryPath { get; }
        = Directory.CreateDirectory(Path.Join(DbContextBackup.OsBasePath, "log")).FullName;

    /// <summary>
    /// Creates a logger configuration for the application.
    /// </summary>
    /// <returns>A Logger object representing the configured logger.</returns>
    public static Logger CreateConfig(LogEventLevel? logEventLevel)
    {
        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.SetWriteToOption(true, true, LogDirectoryPath);
        loggerConfiguration.SetLoggerConfigurationLevel(logEventLevel);
        var logger = loggerConfiguration.CreateLogger();

        return logger;
    }

    /// <summary>
    /// Removes log files that are older than a specified number of days.
    /// </summary>
    /// <param name="maxDay">The maximum age of log files to keep in days.</param>
    /// <returns>The number of log files deleted.</returns>
    public static int RemoveOldLog(int maxDay)
    {
        var today = DateTime.Now;

        var deletedCount = 0;
        var files = Directory.GetFiles(LogDirectoryPath);
        foreach (var file in files)
        {
            var fileCreationTime = File.GetCreationTime(file);
            var isToOld = fileCreationTime.AddDays(maxDay) < today;

            if (!isToOld) continue;
            try
            {
                File.Delete(file);

                if (!File.Exists(file))  deletedCount += 1;
            }
            catch (Exception)
            {
                // TODO work
                // log the exception or handle it
            }
        }

        return deletedCount;
    }
}