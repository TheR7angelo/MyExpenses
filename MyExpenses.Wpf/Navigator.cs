using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MyExpenses.Wpf;

public static class Navigator
{
    public static Dictionary<Type, Page> Pages { get; } = new();

    private static readonly Dictionary<string, NavigationService> NavigationServices = new();

    static Navigator()
    {
        var mainWindow = (MainWindow)Application.Current.MainWindow!;
        var frame = mainWindow.FrameBody;
        frame.RegisterFrame();
    }

    public static void RegisterFrame(this Frame frame)
    {
        // var duplicateKey = NavigationServices.ContainsKey(frame.Name);
        // if (duplicateKey)
        // {
        //     throw new ArgumentException("Unable to register multiple frames with the same name");
        // }

        NavigationServices[frame.Name] = frame.NavigationService;
    }

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

        var isCache = Pages.TryGetValue(type, out var page);
        if (!isCache)
        {
            page = Activator.CreateInstance(type) as Page;
        }

        Pages[type] = page!;

        NavigationServices[nameOfFrame].Navigate(page);
    }

    public static void NavigateTo(this Frame frame, Type type)
        => frame.Name.NavigateTo(type);

    public static void GoBack(this string nameOfFrame)
    {
        if (NavigationServices[nameOfFrame].CanGoBack) NavigationServices[nameOfFrame].GoBack();
    }

    public static void GoForward(this string nameOfFrame)
    {
        if (NavigationServices[nameOfFrame].CanGoForward) NavigationServices[nameOfFrame].GoForward();
    }
}