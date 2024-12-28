namespace MyExpenses.Wpf;

public class NavigatorEventArgs : EventArgs
{
    public bool CanGoBack { get; init; }
    public bool CanGoForward { get; init; }
}