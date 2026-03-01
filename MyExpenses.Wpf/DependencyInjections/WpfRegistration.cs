using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace MyExpenses.Wpf.DependencyInjections;

public static class WpfRegistration
{
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
}