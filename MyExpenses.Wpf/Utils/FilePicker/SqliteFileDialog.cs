using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Utils.FilePicker;

public class SqliteFileDialog(string? titleOpenFile = null, string? titleSaveFile = null, bool multiSelect = false, string? defaultFileName=null)
    : AFileDialog(titleOpenFile, titleSaveFile, multiSelect, Extensions, "Fichier base donnée sqlite", defaultFileName)
{
    private new static IEnumerable<string> Extensions => [DbContextBackup.Extension];
}