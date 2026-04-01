using MyExpenses.Presentation.Enums;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IDialogService
{
    /// <summary>
    /// Displays an input dialog with a specified title, message, and configuration for user input.
    /// </summary>
    /// <param name="title">The title of the input dialog.</param>
    /// <param name="message">The message or prompt to display in the input dialog.</param>
    /// <param name="inputResult">The result of the user's interaction with the dialog, indicating the selected action.</param>
    /// <param name="input">The input entered by the user, or null if no input was provided.</param>
    /// <param name="maxLength">The maximum allowed length of the input. A value of 0 means no limit.</param>
    /// <param name="placeholder">The placeholder or hint text to display in the input textbox when empty.</param>
    /// <returns>True if the user provided input and confirmed the dialog; otherwise, false.</returns>
    public bool ShowInputDialog(string title, string message, out MessageBoxInputResult inputResult, out string? input,
        int maxLength = 0, string placeholder = "");

    /// <summary>
    /// Displays a message box with a specified caption, text, and icon, and returns the user's response.
    /// </summary>
    /// <param name="caption">The title or caption of the message box.</param>
    /// <param name="messageBoxText">The text or message to display in the message box.</param>
    /// <param name="icon">The icon to display in the message box, indicating the type of message (e.g., error, warning, information).</param>
    /// <returns>The result of the user's interaction with the message box, represented as a <see cref="MessageBoxResult"/> value.</returns>
    public MessageBoxResult ShowMessageBox(string caption, string messageBoxText, MsgBoxImage icon);

    /// <summary>
    /// Displays a message box with the specified caption, text, button options, and icon.
    /// </summary>
    /// <param name="caption">The caption to display in the title bar of the message box.</param>
    /// <param name="messageBoxText">The text to display in the body of the message box.</param>
    /// <param name="button">The button options to include in the message box, such as OK, Cancel, Yes, or No.</param>
    /// <param name="icon">The icon to display in the message box, indicating the type or context of the message.</param>
    /// <returns>A <see cref="MessageBoxResult"/> value indicating the button selected by the user.</returns>
    public MessageBoxResult ShowMessageBox(string caption, string messageBoxText, MessageBoxButton button, MsgBoxImage icon);
}