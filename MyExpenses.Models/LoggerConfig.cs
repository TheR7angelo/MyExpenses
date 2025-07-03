using Microsoft.Extensions.Logging;
using MyExpenses.Models.Systems;
using MyExpenses.SharedUtils.GlobalInfos;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace MyExpenses.Models;

/// <summary>
/// Provides methods for configuring the logger.
/// </summary>
public static class LoggerConfig
{
    public static ILoggerFactory? LoggerFactory { get; private set; }
    public static bool LogEfCore { get; private set; }

    /// <summary>
    /// Creates a logger configuration for the application.
    /// </summary>
    /// <returns>A Logger object representing the configured logger.</returns>
    public static Logger CreateConfig(LogEventLevel? logEventLevel, bool logEfCore, bool writeToFileEfCore)
    {
        LogEfCore = logEfCore;

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The LoggerConfiguration instance is created here to configure and build the logger.
        var loggerConfiguration = new LoggerConfiguration();

        loggerConfiguration.SetWriteToOption(true, true, OsInfos.LogDirectoryPath);
        loggerConfiguration.SetLoggerConfigurationLevel(logEventLevel);
        var logger = loggerConfiguration.CreateLogger();

        LoggerFactory = new LoggerFactory().AddSerilog(logger);

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
        var files = Directory.GetFiles(OsInfos.LogDirectoryPath);
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