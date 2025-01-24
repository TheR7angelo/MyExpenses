using MyExpenses.SharedUtils.GlobalInfos;
using MyExpenses.Wpf.Resources.Resx.Utils.FilePicker.SqliteFileDialog;

namespace MyExpenses.Wpf.Utils.FilePicker;

public class SqliteFileDialog(
    string? titleOpenFile = null,
    string? titleSaveFile = null,
    bool multiSelect = false,
    string? defaultFileName = null)
    : AFileDialog(titleOpenFile, titleSaveFile, multiSelect, [DatabaseInfos.Extension],
        SqliteFileDialogResources.FilterText, defaultFileName);