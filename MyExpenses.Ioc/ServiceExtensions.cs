using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.AutoMapper;
using MyExpenses.Application.DbStateProviders;
using MyExpenses.Sql.Context;
using Serilog.Events;

namespace MyExpenses.Ioc;

/// <summary>
/// Provides extension methods to configure application services for different application platforms.
/// </summary>
public static class ServiceExtensions
{
    private static void AddCommonServices(this IServiceCollection services, LogEventLevel logEventLevel = LogEventLevel.Information)
    {
        services.AddServiceLogging(logEventLevel);

        services.AddSingleton<IDbStateProvider, DbStateProvider>();
        services.AddAutoMapper(_ => { },
            typeof(MappingProfile).Assembly,
            typeof(MyExpenses.Infrastructure.AutoMapper.MappingProfile).Assembly);

        services.AddDbContext<DataBaseContext>((serviceProvider, options) =>
        {
            var stateProvider = serviceProvider.GetRequiredService<IDbStateProvider>();
            if (stateProvider.FilePath is null)
            {
                throw new InvalidOperationException("Database file path is null");
            }

            var connectionString =
                stateProvider.FilePath.BuildConnectionString(pooling: false,
                    mode: SqliteOpenMode.ReadWrite);

            options.UseSqlite(connectionString);

            #if DEBUG

            var loggerFactory =serviceProvider.GetRequiredService<ILoggerFactory>();
            options.UseLoggerFactory(loggerFactory)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();

            #endif
        });
    }

    /// <summary>
    /// Configures and registers application services specific to a WPF application platform.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    public static void AddWpfServices(this IServiceCollection services)
    {
        services.AddCommonServices();
    }

    /// <summary>
    /// Configures and registers application services specific to a .NET MAUI application platform.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    public static void AddMauiServices(this IServiceCollection services)
    {
        services.AddCommonServices();
    }
}