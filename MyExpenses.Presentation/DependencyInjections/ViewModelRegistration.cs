using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Presentation.Services;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.Validations;
using MyExpenses.Presentation.Validations.Interfaces;
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
    /// <param name="services">
    /// An instance of <see cref="IServiceCollection"/> where the view model services will be registered.
    /// </param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers the view models required for the application with the dependency injection container.
        /// </summary>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> with the view model services added.
        /// </returns>
        public IServiceCollection RegisterViewModels()
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

        /// <summary>
        /// Registers validation services required for the application with the dependency injection container.
        /// </summary>
        /// <returns>
        /// The updated <see cref="IServiceCollection"/> with the validation services added.
        /// </returns>
        public IServiceCollection RegisterValidationServices()
        {
            services.AddScoped<IAccountPresentationValidationService, AccountPresentationValidationService>()
                .AddScoped<IExpensePresentationValidationService, ExpensePresentationValidationService>()
                .AddScoped<ISystemPresentationValidationService, SystemPresentationValidationService>();

            return services;
        }

        public IServiceCollection RegisterActionServices()
        {
            services.AddTransient<IAccountActionService, AccountActionService>()
                .AddTransient<IExpenseActionService, ExpenseActionService>()
                .AddTransient<ISystemActionService, SystemActionService>();

            return services;
        }
    }
}