using System.Reflection;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Presentation.Services.Interfaces;

namespace MyExpenses.Wpf;

public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _routes = new();
    private System.Windows.Navigation.NavigationService? _navigationService;

    private bool _canGoBack;
    private bool _canGoForward;

    public bool CanGoBack
    {
        get => _canGoBack;
        private set
        {
            if (_canGoBack == value) return;

            _canGoBack = value;
            CanGoBackChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanGoForward
    {
        get => _canGoForward;
        private set
        {
            if (_canGoForward == value)
                return;

            _canGoForward = value;
            CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? CanGoBackChanged;

    public event EventHandler? CanGoForwardChanged;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        RegisterRoutesFromAssembly();
    }

    private void RegisterRoutesFromAssembly()
    {
        var assembly = Assembly.GetExecutingAssembly();

        var pageTypes = assembly
            .GetTypes()
            .Where(type =>
                type is { IsClass: true, IsAbstract: false } &&
                type.IsSubclassOf(typeof(Page)));

        foreach (var pageType in pageTypes)
        {
            _routes[pageType.Name] = pageType;
        }
    }

    public void Initialize(Frame frame)
    {
        _navigationService = frame.NavigationService;
        _navigationService.Navigated += OnNavigated;

        UpdateNavigationState();
    }

    public void Navigate(string route, object? parameter = null)
    {
        if (_navigationService is null)
            throw new InvalidOperationException("The navigation service has not been initialized.");

        if (!_routes.TryGetValue(route, out var pageType))
        {
            throw new ArgumentException($@"Unknown navigation route: {route}", nameof(route));
        }

        var page = _serviceProvider.GetRequiredService(pageType);

        _navigationService.Navigate(page, parameter);

        UpdateNavigationState();
    }

    public void GoBack()
    {
        if (_navigationService is null)
        {
            throw new InvalidOperationException("The navigation service has not been initialized.");
        }

        if (_navigationService.CanGoBack) _navigationService.GoBack();

        UpdateNavigationState();
    }

    public void GoForward()
    {
        if (_navigationService is null)
        {
            throw new InvalidOperationException("The navigation service has not been initialized.");
        }

        if (_navigationService.CanGoForward) _navigationService.GoForward();

        UpdateNavigationState();
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        UpdateNavigationState();
    }

    private void UpdateNavigationState()
    {
        if (_navigationService is null)
        {
            CanGoBack = false;
            CanGoForward = false;
            return;
        }

        CanGoBack = _navigationService.CanGoBack;
        CanGoForward = _navigationService.CanGoForward;
    }
}