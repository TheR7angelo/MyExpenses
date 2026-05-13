using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Extensions.DependencyInjection;
using MyExpenses.Presentation.Services.Interfaces;

namespace MyExpenses.Wpf;

/// <summary>
/// Service providing navigation logic for WPF applications.
/// Manages page routing, browser-style history, and global cursor states during transitions.
/// </summary>
public sealed class NavigationService : INavigationService
{
    /// <summary>
    /// Instance of <see cref="IServiceProvider"/> used to resolve dependencies or retrieve
    /// registered services dynamically during runtime. Commonly used for instantiating
    /// navigation target pages and their associated dependencies in the navigation process.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Dictionary that maps route names to their corresponding <see cref="Type"/> representations
    /// for pages in the WPF application. Used internally for resolving and navigating to pages
    /// based on their registered routes.
    /// </summary>
    private readonly Dictionary<string, Type> _routes = new();

    /// <summary>
    /// Internal instance of <see cref="System.Windows.Navigation.NavigationService"/> used
    /// to manage page navigation within a WPF application. Handles operations such as
    /// navigating to pages, managing navigation history, and triggering events during
    /// navigation processes.
    /// </summary>
    private System.Windows.Navigation.NavigationService? _navigationService;

    /// <summary>
    /// Indicates whether there is at least one entry in the navigation back stack,
    /// allowing the user to return to the previous page or state. Updates dynamically
    /// based on the current navigation history managed by the service.
    /// </summary>
    public bool CanGoBack
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            CanGoBackChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Indicates whether there is a forward navigation entry in the navigation stack.
    /// This property is updated dynamically based on the current state of the navigation service
    /// and reflects whether the application can navigate to a forward entry.
    /// </summary>
    public bool CanGoForward
    {
        get;
        private set
        {
            if (field == value) return;
            field = value;
            CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Event triggered when the value of <see cref="NavigationService.CanGoBack"/> changes.
    /// This event notifies subscribers about the updated navigation state, enabling UI
    /// components or other listeners to react to the change in backward navigation capability.
    /// </summary>
    public event EventHandler? CanGoBackChanged;

    /// <summary>
    /// Event triggered when the value of <see cref="CanGoForward"/> property changes.
    /// Notifies subscribers about the ability to navigate forward in the navigation history,
    /// enabling them to react to changes in navigation state.
    /// </summary>
    public event EventHandler? CanGoForwardChanged;

    /// <summary>
    /// Service providing navigation logic for WPF applications.
    /// Manages page routing, browser-style history, and global cursor states during transitions.
    /// </summary>
    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        RegisterRoutesFromAssembly();
    }

    /// <summary>
    /// Registers navigation routes by scanning the executing assembly for classes
    /// that inherit from the <see cref="Page"/> class.
    /// Populates the internal dictionary with route names and their associated types.
    /// </summary>
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

    /// <summary>
    /// Initializes the <see cref="NavigationService"/> with the specified frame,
    /// enabling navigation and updating the internal state of navigation controls.
    /// Sets up event handlers for navigational events of the frame.
    /// </summary>
    /// <param name="frame">The <see cref="Frame"/> to be used for managing navigation within the application.</param>
    public void Initialize(Frame frame)
    {
        _navigationService = frame.NavigationService;
        _navigationService.Navigated += OnNavigated;
        _navigationService.Navigating += OnNavigating;
        _navigationService.LoadCompleted += OnLoadCompleted;

        UpdateNavigationState();
    }

    /// <summary>
    /// Handles the LoadCompleted event. Note: using 'async void' is acceptable for event handlers.
    /// </summary>
    private async void OnLoadCompleted(object sender, NavigationEventArgs e)
        => await SetBusyState(false);

    /// <summary>
    /// Handles the Navigating event.
    /// </summary>
    private async void OnNavigating(object sender, NavigatingCancelEventArgs e)
        => await SetBusyState(true);

    /// <summary>
    /// Navigates to a registered route.
    /// </summary>
    public async void Navigate(string route, object? parameter = null)
    {
        if (_navigationService is null)
            throw new InvalidOperationException("The navigation service has not been initialized.");

        if (!_routes.TryGetValue(route, out var pageType))
        {
            throw new ArgumentException($@"Unknown navigation route: {route}", nameof(route));
        }

        try
        {
            // On attend que l'état "occupé" soit appliqué
            await SetBusyState(true);

            var page = _serviceProvider.GetRequiredService(pageType);
            _navigationService.Navigate(page, parameter);
            UpdateNavigationState();
        }
        catch
        {
            await SetBusyState(false);
            throw;
        }
    }

    /// <summary>
    /// Navigates back to the previous entry in the navigation history, if possible.
    /// Raises the CanGoBackChanged event when the navigation state changes.
    /// </summary>
    public void GoBack()
    {
        if (_navigationService?.CanGoBack is not true) return;
        _navigationService.GoBack();
        UpdateNavigationState();
    }

    /// <summary>
    /// Navigates to the next page in the navigation history, if one exists.
    /// Updates the navigation state after completing the operation.
    /// </summary>
    public void GoForward()
    {
        if (_navigationService?.CanGoForward is not true) return;
        _navigationService.GoForward();
        UpdateNavigationState();
    }

    /// <summary>
    /// Handles post-navigation logic, including updating the application state and resetting the busy state.
    /// </summary>
    /// <param name="sender">The source of the navigation event, typically the navigation service.</param>
    /// <param name="e">The navigation event arguments containing details of the navigation.</param>
    private async void OnNavigated(object sender, NavigationEventArgs e)
    {
        await SetBusyState(false);
        UpdateNavigationState();
    }

    /// <summary>
    /// Updates the navigation state by determining whether backward and forward navigation is possible.
    /// Updates the CanGoBack and CanGoForward properties accordingly.
    /// </summary>
    private void UpdateNavigationState()
    {
        if (_navigationService is null) return;
        CanGoBack = _navigationService.CanGoBack;
        CanGoForward = _navigationService.CanGoForward;
    }

    /// <summary>
    /// Manages the visual busy state of the application.
    /// </summary>
    /// <param name="isBusy">True to show the wait cursor; False to restore default.</param>
    private async Task SetBusyState(bool isBusy)
    {
        if (isBusy)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (System.Windows.Application.Current.MainWindow is not null) System.Windows.Application.Current.MainWindow.IsHitTestVisible = false;
        }
        else
        {
            await Task.Delay(TimeSpan.FromMilliseconds(50));
            Mouse.OverrideCursor = null;
            if (System.Windows.Application.Current.MainWindow is not null) System.Windows.Application.Current.MainWindow.IsHitTestVisible = true;
        }
    }
}