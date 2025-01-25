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

    public IEnumerable<string> Extensions { get; } = Array.Empty<string>();

    public string[]? GetFiles()
    {
        var path = GetFile();

        if (string.IsNullOrEmpty(path)) return null;
        return [path];
    }

    public string? GetFile()
        => GetFiles()?.FirstOrDefault();

    public string? SaveFile()
        => null;
}