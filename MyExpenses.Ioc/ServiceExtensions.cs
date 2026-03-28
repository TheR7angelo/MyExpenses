using Domain.Interfaces;
using Domain.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.DbStateProviders;
using MyExpenses.Application.Interfaces.IRepositories;
using MyExpenses.Application.Interfaces.IServices;
using MyExpenses.Application.Interfaces.Mappings;
using MyExpenses.Infrastructure.Mapping;
using MyExpenses.Infrastructure.Services;
using MyExpenses.Presentation.Mappings;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
using MyExpenses.Presentation.Validations.Interfaces;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Repositories;
using MyExpenses.Sql.Validations;
using Serilog.Events;

namespace MyExpenses.Ioc;

/// <summary>
/// Provides extension methods to configure application services for different application platforms.
/// </summary>
public static class ServiceExtensions
{
    private static IServiceCollection AddCommonServices(this IServiceCollection services, LogEventLevel logEventLevel = LogEventLevel.Information)
    {
        services.AddScoped<IAccountRepository, AccountRepository>()
            .AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddScoped<IAccountValidationRepository, AccountValidationRepository>();

        services.AddScoped<IAccountService, AccountService>()
            .AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<IAccountDomainValidationService, AccountDomainValidationService>();

        services.AddScoped<IAccountPresentationService, AccountPresentationService>()
            .AddScoped<ICategoryPresentationService, CategoryPresentationService>();

        services.AddScoped<IAccountPresentationValidationService, AccountPresentationValidationService>();

        services.AddSingleton<IAccountDtoDomainMapper, AccountDtoDomainMapper>()
            .AddSingleton<ICategoryDtoDomainMapper, CategoryDtoDomainMapper>();

        services.AddSingleton<IAccountDtoViewModelMapper, AccountDtoViewModelMapper>()
            .AddSingleton<ICategoryDtoViewModelMapper, CategoryDtoViewModelMapper>();

        services.AddServiceLogging(logEventLevel);

        services.AddSingleton<IDbStateProvider, DbStateProvider>();

        services.AddDbContext<DataBaseContext>(ConfigureDatabase, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);
        services.AddDbContextFactory<DataBaseContext>(ConfigureDatabase);

        return services;
    }

    private static void ConfigureDatabase(IServiceProvider serviceProvider, DbContextOptionsBuilder options)
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
    }

    /// <summary>
    /// Configures and registers application services specific to a WPF application platform.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    public static IServiceCollection AddWpfServices(this IServiceCollection services)
    {
        services.AddCommonServices();
        return services;
    }

    /// <summary>
    /// Configures and registers application services specific to a .NET MAUI application platform.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    public static IServiceCollection AddMauiServices(this IServiceCollection services)
    {
        services.AddCommonServices();
        return services;
    }
}