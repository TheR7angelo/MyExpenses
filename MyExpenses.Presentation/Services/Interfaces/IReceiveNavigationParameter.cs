namespace MyExpenses.Presentation.Services.Interfaces;

/// <summary>
/// Interface that a page can implement to receive navigation parameters directly.
/// This ensures that parameters are received reliably regardless of timing.
/// </summary>
public interface IReceiveNavigationParameter
{
    /// <summary>
    /// Called when the page receives a navigation parameter.
    /// </summary>
    /// <param name="parameter">The parameter passed during navigation, can be null</param>
    void OnNavigationParameterReceived(object? parameter);
}

