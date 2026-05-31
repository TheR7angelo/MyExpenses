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
using MyExpenses.Presentation.DependencyInjections;
using MyExpenses.Presentation.Mappings;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Repositories;
using MyExpenses.Sql.Validations;
using MyExpenses.WebApi.DependencyInjections;
using MyExpenses.WebApi.Repositories;
using Serilog.Events;

namespace MyExpenses.Ioc;

/// <summary>
/// Provides extension methods to configure application services for different application platforms.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Configures and registers common application services including repositories, services, mappers, database contexts,
    /// and logging for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="logEventLevel">Specifies the minimum level of log events to be captured by the logging system.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the registered services.</returns>
    public static IServiceCollection AddCommonServices(this IServiceCollection services,
        LogEventLevel logEventLevel = LogEventLevel.Information)
    {
        services.AddScoped<IAccountRepository, AccountRepository>()
            .AddScoped<IExpenseRepository, ExpenseRepository>()
            .AddScoped<ISystemRepository, SystemRepository>()
            .AddScoped<ILocationRepository, LocationRepository>()
            .AddScoped<INominatimRepository, NominatimRepository>();

        services.AddScoped<IAccountValidationRepository, AccountValidationRepository>()
            .AddScoped<IExpenseValidationRepository, ExpenseValidationRepository>();

        services.AddScoped<IAccountService, AccountService>()
            .AddScoped<IExpenseService, ExpenseService>()
            .AddScoped<ISystemService, SystemService>()
            .AddScoped<ILocationService, LocationService>()
            .AddScoped<INominatimService, NominatimService>();

        services.AddScoped<IAccountDomainValidationService, AccountDomainValidationService>();

        services.AddScoped<IAccountPresentationService, AccountPresentationService>()
            .AddScoped<IExpensePresentationService, ExpensePresentationService>()
            .AddScoped<ISystemPresentationService, SystemPresentationService>()
            .AddScoped<ILocationPresentationService, LocationPresentationService>();

        services.AddScoped<IAccountDtoDomainMapper, AccountDtoDomainMapper>()
            .AddScoped<IExpenseDtoDomainMapper, ExpenseDtoDomainMapper>()
            .AddScoped<ISystemDtoDomainMapper, SystemDtoDomainMapper>()
            .AddScoped<ILocationDtoDomainMapper, LocationDtoDomainMapper>()
            .AddScoped<INominatimDtoDomainMapper, NominatimDtoDomainMapper>();

        services.AddScoped<IAccountDtoViewModelMapper, AccountDtoViewModelMapper>()
            .AddScoped<IExpenseDtoViewModelMapper, ExpenseDtoViewModelMapper>()
            .AddScoped<ISystemDtoViewModelMapper, SystemDtoViewModelMapper>()
            .AddScoped<ILocationDtoViewModelMapper, LocationDtoViewModelMapper>()
            .AddScoped<INominatimDtoViewModelMapper, NominatimDtoViewModelMapper>();

        services.AddServiceLogging(logEventLevel);

        services.AddSingleton<IDbStateProvider, DbStateProvider>();

        services.AddDbContext<DataBaseContext>(ConfigureDatabase, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);
        services.AddDbContextFactory<DataBaseContext>(ConfigureDatabase);

        services.AddHttpClientBuilder();

        services.RegisterViewModels()
            .RegisterValidationServices()
            .RegisterActionServices()
            .RegisterValidator()
            .RegisterHelper();

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

#if DEBUG

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        options.UseLoggerFactory(loggerFactory)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
#else
        options.UseLoggerFactory(new Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory());
        options.ConfigureWarnings(w => w.Ignore());
#endif
        options.UseSqlite(connectionString);
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