using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MyExpenses.Wpf.Helper;

/// <summary>
/// The Navigator class provides navigation functionality for frames within a Windows Presentation Foundation (WPF) application.
/// </summary>
public static class Navigator
{
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
    }

    public static void NavigateTo(this Frame frame, string path, object? param = null)
        => frame.Name.NavigateTo(path, param);

    public static void NavigateTo(this string nameOfFrame, Page page)
    {
        NavigationServices[nameOfFrame].Navigate(page);
    }

    public static void NavigateTo(this Frame frame, Page page)
        => frame.Name.NavigateTo(page);

    public static void NavigateTo(this string nameOfFrame, Type type)
    {
        if (!type.IsSubclassOf(typeof(Page)))
        {
            throw new ArgumentException("Type must be a subclass of Page");
        }

        var page = Activator.CreateInstance(type) as Page;

        NavigationServices[nameOfFrame].Navigate(page);
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