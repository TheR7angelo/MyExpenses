using Ookii.Dialogs.Wpf;

namespace MyExpenses.Wpf.Utils.FilePicker;

/// <summary>
/// Represents a dialog for selecting folders or files.
/// </summary>
public class FolderDialog(string? titleOpenFile = null, bool multiSelect = false)
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    // Creates an instance of VistaFolderBrowserDialog to handle folder selection operations.
    private readonly VistaFolderBrowserDialog _vistaFolderBrowserDialog = new()
    {
        Description = titleOpenFile,
        UseDescriptionForTitle = true,
        Multiselect = multiSelect
    };

    /// <summary>
    /// Retrieves all selected files from the folder picker dialog.
    /// </summary>
    /// <returns>An array of strings containing the paths of selected files, or null if no files were selected.</returns>
    private string[]? GetFiles()
    {
        var success = _vistaFolderBrowserDialog.ShowDialog();
        if(success is not true) return null;
        var path = _vistaFolderBrowserDialog.SelectedPaths;

        return path.Length is 0 ? null : path;
    }

    /// <summary>
    /// Retrieves the first selected file from the file picker dialog.
    /// </summary>
    /// <returns>The path of the first selected file as a string, or null if no file was selected.</returns>
    public string? GetFile()
        => GetFiles()?.FirstOrDefault();
}