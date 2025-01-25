using Ookii.Dialogs.Wpf;

namespace MyExpenses.Wpf.Utils.FilePicker;

public abstract class AFileDialog
{
    private VistaOpenFileDialog? _vistaOpenFileDialog;

    private VistaOpenFileDialog VistaOpenFileDialog
    {
        get
        {
            if (_vistaOpenFileDialog is null) InitializeOpenFileDialog();
            return _vistaOpenFileDialog!;
        }
    }

    private VistaSaveFileDialog? _vistaSaveFileDialog;

    private VistaSaveFileDialog VistaSaveFileDialog
    {
        get
        {
            if (_vistaSaveFileDialog is null) InitializeVistaSaveFileDialog();
            return _vistaSaveFileDialog!;
        }
    }

    private string? TitleOpenFile { get; }
    private string? TitleSaveFile { get; }
    private bool Multiselect { get; }
    private string? DefaultFileName { get; }
    private string Filter { get; }
    private string FilterText { get; }
    private IEnumerable<string> Extensions { get; }

    protected AFileDialog(string? titleOpenFile, string? titleSaveFile, bool multiselect,
        IEnumerable<string> extensions, string filterText, string? defaultFileName=null)
    {
        TitleOpenFile = titleOpenFile;
        TitleSaveFile = titleSaveFile;
        Multiselect = multiselect;
        DefaultFileName = defaultFileName;

        Extensions = extensions;
        FilterText = filterText;

        Filter = GetFilter();
    }

    private void InitializeVistaSaveFileDialog()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Creates an instance of VistaSaveFileDialog to handle file saving operations.
        _vistaSaveFileDialog = new VistaSaveFileDialog
        {
            Title = TitleSaveFile,
            Filter = Filter,
            FileName = DefaultFileName
        };
    }

    private void InitializeOpenFileDialog()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Creates an instance of VistaOpenFileDialog to handle file opening operations.
        _vistaOpenFileDialog = new VistaOpenFileDialog
        {
            Title = TitleOpenFile,
            Multiselect = Multiselect,
            Filter = Filter
        };
    }

    private string GetFilter()
    {
        var lst = string.Join(';', Extensions.Select(ext => $"*{ext}"));
        return $"{FilterText} ({lst})|{lst}";
    }

    public string[]? GetFiles()
    {
        var result = VistaOpenFileDialog.ShowDialog();

        if (result is true)
        {
            return VistaOpenFileDialog.Multiselect ? VistaOpenFileDialog.FileNames : [VistaOpenFileDialog.FileName];
        }

        return null;
    }

    public string? GetFile()
        => GetFiles()?.FirstOrDefault();

    public string? SaveFile()
    {
        var result = VistaSaveFileDialog.ShowDialog();
        return result is true ? VistaSaveFileDialog.FileName : null;
    }
}