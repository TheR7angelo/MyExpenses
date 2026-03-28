using MyExpenses.Presentation.Enums;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IDialogService
{
    /// <summary>
    /// Displays an input dialog with a specified title, message, and configuration for user input.
    /// </summary>
    /// <param name="title">The title of the input dialog.</param>
    /// <param name="message">The message or prompt to display in the input dialog.</param>
    /// <param name="result">The result of the user's interaction with the dialog, indicating the selected action.</param>
    /// <param name="input">The input entered by the user, or null if no input was provided.</param>
    /// <param name="maxLength">The maximum allowed length of the input. A value of 0 means no limit.</param>
    /// <param name="placeholder">The placeholder or hint text to display in the input textbox when empty.</param>
    /// <returns>True if the user provided input and confirmed the dialog; otherwise, false.</returns>
    public bool ShowInputDialog(string title, string message, out MessageBoxResult result, out string? input,
        int maxLength = 0, string placeholder = "");
}