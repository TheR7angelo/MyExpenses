using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Application.AutoMapper;
using MyExpenses.Application.DbStateProviders;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Sql.Context;

namespace MyExpenses.Ioc;

/// <summary>
/// Provides extension methods to configure application services for different application platforms.
/// </summary>
public static class ServiceExtensions
{
    private static void AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbStateProvider, DbStateProvider>();
        services.AddAutoMapper(_ => { },
            typeof(MappingProfile).Assembly,
            typeof(MyExpenses.Infrastructure.AutoMapper.MappingProfile).Assembly);

        services.AddDbContext<DataBaseContext>((serviceProvider, options) =>
        {
            var stateProvider = serviceProvider.GetRequiredService<IDbStateProvider>();
            options.UseSqlite(stateProvider.CurrentConnectionString);
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