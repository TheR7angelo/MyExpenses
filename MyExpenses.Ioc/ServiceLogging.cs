using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyExpenses.Models.Systems;
using MyExpenses.SharedUtils.GlobalInfos;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace MyExpenses.Ioc;

public static class ServiceLogging
{
    private const string Template = "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}";
    private static readonly string DefaultFilename = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";

    public static IServiceCollection AddServiceLogging(this IServiceCollection services, LogEventLevel logEventLevel)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(logEventLevel)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: Template, theme: AnsiConsoleTheme.Code, applyThemeToRedirectedOutput: true)
            .WriteTo.File(Path.Join(OsInfos.LogDirectoryPath, DefaultFilename), outputTemplate: Template, rollingInterval: RollingInterval.Day);

        var logger = loggerConfiguration.CreateLogger();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(logger, dispose: true);
        });

        return services;
    }
}