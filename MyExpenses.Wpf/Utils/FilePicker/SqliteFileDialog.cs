using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Utils.FilePicker;

// TODO trad
public class SqliteFileDialog(
    string? titleOpenFile = null,
    string? titleSaveFile = null,
    bool multiSelect = false,
    string? defaultFileName = null)
    : AFileDialog(titleOpenFile, titleSaveFile, multiSelect, [DbContextBackup.Extension],
        "Fichier base donnée sqlite", defaultFileName)
{
}