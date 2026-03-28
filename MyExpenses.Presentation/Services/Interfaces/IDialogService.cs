using MyExpenses.Presentation.Enums;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IDialogService
{
    /// <summary>
    /// Displays a dialog with an input field and returns user input along with the dialog result.
    /// </summary>
    /// <param name="title">The title of the dialog window.</param>
    /// <param name="message">The message or prompt displayed within the dialog.</param>
    /// <param name="result">An output parameter that captures the result of the dialog (e.g., None, Cancel, Delete, Valid).</param>
    /// <param name="input">An output parameter that captures the user's input from the dialog.</param>
    /// <param name="maxLength">An optional parameter that specifies the maximum allowed length of the input field. Default is 0, which indicates no limit.</param>
    /// <returns>Returns true if the dialog was confirmed (e.g., OK or valid action); otherwise, false.</returns>
    public bool ShowInputDialog(string title, string message, out MessageBoxResult result, out string? input,
        int maxLength = 0);
}