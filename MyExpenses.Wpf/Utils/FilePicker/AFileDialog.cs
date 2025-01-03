using Ookii.Dialogs.Wpf;

namespace MyExpenses.Wpf.Utils.FilePicker;

public abstract class AFileDialog
{
    private string FilterText { get; }
    public virtual IEnumerable<string> Extensions { get; }

    private readonly VistaOpenFileDialog _vistaOpenFileDialog;
    private readonly VistaSaveFileDialog _vistaSaveFileDialog;

    protected AFileDialog(string? titleOpenFile, string? titleSaveFile, bool multiselect,
        IEnumerable<string> extensions, string filterText, string? defaultFileName=null)
    {
        Extensions = extensions;
        FilterText = filterText;

        var filter = GetFilter();

        _vistaOpenFileDialog = new VistaOpenFileDialog
        {
            Title = titleOpenFile,
            Multiselect = multiselect,
            Filter = filter
        };

        _vistaSaveFileDialog = new VistaSaveFileDialog
        {
            Title = titleSaveFile,
            Filter = filter,
            FileName = defaultFileName
        };
    }

    private string GetFilter()
    {
        var lst = string.Join(';', Extensions.Select(ext => $"*{ext}"));
        return $"{FilterText} ({lst})|{lst}";
    }

    public string[]? GetFiles()
    {
        var result = _vistaOpenFileDialog.ShowDialog();

        if (result is true)
        {
            return _vistaOpenFileDialog.Multiselect ? _vistaOpenFileDialog.FileNames : [_vistaOpenFileDialog.FileName];
        }

        return null;
    }

    public string? GetFile()
    {
        var result = _vistaOpenFileDialog.ShowDialog();

        if (result is true)
        {
            return _vistaOpenFileDialog.Multiselect
                ? _vistaOpenFileDialog.FileNames.First()
                : _vistaOpenFileDialog.FileName;
        }

        return null;
    }

    public string? SaveFile()
    {
        var result = _vistaSaveFileDialog.ShowDialog();
        return result is true ? _vistaSaveFileDialog.FileName : null;
    }
}