namespace MyExpenses.Wpf;

public class NavigatorEventArgs : EventArgs
{
    public bool CanGoBack { get; set; }
    public bool CanGoForward { get; set; }
}