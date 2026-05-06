using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Presentation.DependencyInjections;

/// <summary>
/// Provides extension methods for registering view models into the dependency injection container.
/// </summary>
/// <remarks>
/// The <c>ViewModelRegistration</c> class contains methods that facilitate the registration of
/// view model classes required by the MyExpenses application. These view models implement core
/// features using the MVVM pattern and are injected into application components to enable
/// separation of concerns and simplify testing.
/// </remarks>
public static class ViewModelRegistration
{
    /// <summary>
    /// Registers the view models required for the application with the dependency injection container.
    /// </summary>
    /// <param name="services">
    /// An instance of <see cref="IServiceCollection"/> where the view model services will be registered.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with the view model services added.
    /// </returns>
    public static IServiceCollection RegisterViewModels(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var viewModelTypes = assembly.GetTypes()
            .Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                typeof(ViewModelBase).IsAssignableFrom(t));

        foreach (var viewType in viewModelTypes)
        {
            services.AddTransient(viewType);
        }

        return services;
    }
}