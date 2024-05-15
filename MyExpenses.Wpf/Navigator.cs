using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MyExpenses.Wpf;

/// <summary>
/// The Navigator class provides navigation functionality for frames within a Windows Presentation Foundation (WPF) application.
/// </summary>
public static class Navigator
{
    /// <summary>
    /// Occurs when the value of the CanGoBack property has changed.
    /// </summary>
    /// <remarks>
    /// This event is raised when the value of the CanGoBack property changes. The CanGoBack property indicates whether the registered frame can navigate back in the navigation history.
    /// </remarks>
    /// <seealso cref="Navigator.CanGoBack"/>
    /// <seealso cref="Navigator"/>
    /// <seealso cref="NavigatorEventArgs"/>
    /// <seealso cref="EventHandler{TEventArgs}"/>
    public static event EventHandler<NavigatorEventArgs>? CanGoBackChanged;

    /// <summary>
    /// Occurs when the value of the CanGoForward property has changed.
    /// </summary>
    /// <remarks>
    /// This event is raised when the value of the CanGoForward property changes. The CanGoForward property indicates whether the registered frame can navigate forward in the navigation history.
    /// </remarks>
    /// <seealso cref="Navigator.CanGoForward"/>
    /// <seealso cref="Navigator"/>
    /// <seealso cref="NavigatorEventArgs"/>
    /// <seealso cref="EventHandler{TEventArgs}"/>
    public static event EventHandler<NavigatorEventArgs>? CanGoForwardChanged;

    private static bool _canGoBack;
    private static bool _canGoForward;

    /// <summary>
    /// Gets or sets a value indicating whether the registered frame can navigate back in the navigation history.
    /// </summary>
    /// <value>
    /// <c>true</c> if the frame can navigate back; otherwise, <c>false</c>.
    /// </value>
    public static bool CanGoBack
    {
        get => _canGoBack;
        set
        {
            if (_canGoBack == value) return;
            _canGoBack = value;
            CanGoBackChanged?.Invoke(null, new NavigatorEventArgs { CanGoBack = _canGoBack, CanGoForward = _canGoForward});
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the registered frame can navigate forward in the navigation history.
    /// </summary>
    /// <value>
    /// <c>true</c> if the frame can navigate forward; otherwise, <c>false</c>.
    /// </value>
    public static bool CanGoForward
    {
        get => _canGoForward;
        set
        {
            if (_canGoForward == value) return;
            _canGoForward = value;
            CanGoForwardChanged?.Invoke(null, new NavigatorEventArgs { CanGoBack = _canGoBack, CanGoForward = _canGoForward});
        }
    }

    /// <summary>
    /// The Navigator class provides navigation functionality for frames within a Windows Presentation Foundation (WPF) application.
    /// </summary>
    public static Dictionary<Type, Page> Pages { get; } = new();

    private static readonly Dictionary<string, NavigationService> NavigationServices = new();

    /// <summary>
    /// The Navigator class provides navigation functionality for frames within a Windows Presentation Foundation (WPF) application.
    /// </summary>
    static Navigator()
    {
        var mainWindow = (MainWindow)Application.Current.MainWindow!;
        var frame = mainWindow.FrameBody;
        frame.RegisterFrame();
    }

    /// <summary>
    /// Registers a frame for navigation within a Windows Presentation Foundation (WPF) application.
    /// </summary>
    /// <param name="frame">The frame to register.</param>
    public static void RegisterFrame(this Frame frame)
    {
        // var duplicateKey = NavigationServices.ContainsKey(frame.Name);
        // if (duplicateKey)
        // {
        //     throw new ArgumentException("Unable to register multiple frames with the same name");
        // }

        NavigationServices[frame.Name] = frame.NavigationService;
    }

    /// <summary>
    /// Navigates to the specified path within a registered frame.
    /// </summary>
    /// <param name="nameOfFrame">The name of the registered frame.</param>
    /// <param name="path">The path to navigate to.</param>
    /// <param name="param">Optional parameter to pass to the page being navigated to.</param>
    private static void NavigateTo(this string nameOfFrame, string path, object? param = null)
    {
        NavigationServices[nameOfFrame].Navigate(new Uri(path, UriKind.RelativeOrAbsolute), param);
        CanGoBack = true;
    }

    public static void NavigateTo(this Frame frame, string path, object? param = null)
        => frame.Name.NavigateTo(path, param);

    public static void NavigateTo(this string nameOfFrame, Page page)
    {
        NavigationServices[nameOfFrame].Navigate(page);
        CanGoBack = true;
    }

    public static void NavigateTo(this Frame frame, Page page)
        => frame.Name.NavigateTo(page);

    public static void NavigateTo(this string nameOfFrame, Type type)
    {
        if (!type.IsSubclassOf(typeof(Page)))
        {
            throw new ArgumentException("Type must be a subclass of Page");
        }

        var isCache = Pages.TryGetValue(type, out var page);
        if (!isCache)
        {
            page = Activator.CreateInstance(type) as Page;
        }

        Pages[type] = page!;

        NavigationServices[nameOfFrame].Navigate(page);
        CanGoBack = true;
    }

    public static void NavigateTo(this Frame frame, Type type)
        => frame.Name.NavigateTo(type);

    /// <summary>
    /// Navigates the registered frame to the previous page in the navigation history.
    /// </summary>
    /// <param name="nameOfFrame">The name of the registered frame.</param>
    public static void GoBack(this string nameOfFrame)
    {
        if (NavigationServices[nameOfFrame].CanGoBack) NavigationServices[nameOfFrame].GoBack();

        CanGoBack = NavigationServices[nameOfFrame].CanGoBack;
    }

    /// <summary>
    /// Navigates the registered frame to the next page in the navigation history.
    /// </summary>
    /// <param name="nameOfFrame">The name of the registered frame.</param>
    public static void GoForward(this string nameOfFrame)
    {
        if (NavigationServices[nameOfFrame].CanGoForward) NavigationServices[nameOfFrame].GoForward();
    }
}