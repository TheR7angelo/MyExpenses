namespace MyExpenses.Presentation.Services.Interfaces;

public interface INavigationService
{
    /// <summary>
    /// Indicates whether there is at least one entry in the navigation back stack,
    /// allowing the user to return to the previous page or state. Updates dynamically
    /// based on the current navigation history managed by the service.
    /// </summary>
    public bool CanGoBack { get; }

    /// <summary>
    /// Indicates whether there is a forward navigation entry in the navigation stack.
    /// This property is updated dynamically based on the current state of the navigation service
    /// and reflects whether the application can navigate to a forward entry.
    /// </summary>
    public bool CanGoForward { get; }

    public event EventHandler? CanGoBackChanged;

    public event EventHandler? CanGoForwardChanged;

    /// <summary>
    /// Navigates back to the previous entry in the navigation history, if possible.
    /// Raises the CanGoBackChanged event when the navigation state changes.
    /// </summary>
    public void GoBack();

    /// <summary>
    /// Navigates to the next page in the navigation, if one exists.
    /// Updates the navigation state after completing the operation.
    /// </summary>
    public void GoForward();

    /// <summary>
    /// Navigates to a registered route.
    /// </summary>
    public void Navigate(string route, object? parameter = null);
}