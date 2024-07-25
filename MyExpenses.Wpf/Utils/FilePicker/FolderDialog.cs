using MyExpenses.Models.Ui.FilePicker;
using Ookii.Dialogs.Wpf;

namespace MyExpenses.Wpf.Utils.FilePicker;

public class FolderDialog(string? titleOpenFile = null, bool multiSelect = false) : IDialog
{
    private readonly VistaFolderBrowserDialog _vistaFolderBrowserDialog = new()
    {
        Description = titleOpenFile,
        UseDescriptionForTitle = true,
        Multiselect = multiSelect
    };

    public IEnumerable<string> Extensions => Array.Empty<string>();

    public string[]? GetFiles()
    {
        var path = GetFile();

        if (string.IsNullOrEmpty(path)) return null;
        return [path];
    }

    public string? GetFile()
    {
        var result = _vistaFolderBrowserDialog.ShowDialog();
        return result is true ? _vistaFolderBrowserDialog.SelectedPath : null;
    }

    public string? SaveFile()
        => null;
}