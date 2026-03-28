using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Wpf.Windows.Dialogs;

namespace MyExpenses.Wpf.DependencyInjections;

public static class WpfRegistration
{
    /// <summary>
    /// Automatically registers all views (classes inheriting from Window or Page) in the current assembly as transient services within the dependency injection container.
    /// </summary>
    /// <param name="services">The IServiceCollection instance to which the views will be registered.</param>
    /// <returns>Returns the updated IServiceCollection instance with the registered views.</returns>
    public static IServiceCollection AddAutoRegisteredViews(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var viewTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && (typeof(Window).IsAssignableFrom(t) || typeof(Page).IsAssignableFrom(t)))
            .AsEnumerable();

        foreach (var viewType in viewTypes)
        {
            services.AddTransient(viewType);
        }

        return services;
    }

    /// <summary>
    /// Registers the dialog service implementation as a transient service within the dependency injection container.
    /// </summary>
    /// <param name="services">The IServiceCollection instance to which the dialog service will be registered.</param>
    /// <returns>Returns the updated IServiceCollection instance with the registered dialog service.</returns>
    public static IServiceCollection AddAutoRegisteredDialogs(this IServiceCollection services)
    {
        services.AddTransient<IDialogService, DialogService>();
        return services;
    }
}