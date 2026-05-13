namespace MyExpenses.Presentation.Services.Interfaces;

public interface INavigationService
{
    public bool CanGoBack { get; }

    public bool CanGoForward { get; }

    public event EventHandler? CanGoBackChanged;

    public event EventHandler? CanGoForwardChanged;

    public void GoBack();

    public void GoForward();

    public void Navigate(string route, object? parameter = null);
}