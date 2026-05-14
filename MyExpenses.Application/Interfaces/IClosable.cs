namespace MyExpenses.Application.Interfaces;

public interface IClosable
{
    /// <summary>
    /// Closes the current view or window.
    /// </summary>
    public void Close();

    public bool? DialogResult { get; set; }
}