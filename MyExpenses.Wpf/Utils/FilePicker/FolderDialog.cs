using Ookii.Dialogs.Wpf;

namespace MyExpenses.Wpf.Utils.FilePicker;

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

    private string[]? GetFiles()
    {
        var success = _vistaFolderBrowserDialog.ShowDialog();
        if(success is not true) return null;
        var path = _vistaFolderBrowserDialog.SelectedPaths;

        return path.Length is 0 ? null : path;
    }

    public string? GetFile()
        => GetFiles()?.FirstOrDefault();
}