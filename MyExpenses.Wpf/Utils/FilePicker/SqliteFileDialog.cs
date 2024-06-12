namespace MyExpenses.Wpf.Utils.FilePicker;

public class SqliteFileDialog(string? titleOpenFile = null, string? titleSaveFile = null, bool multiSelect = false)
    : AFileDialog(titleOpenFile, titleSaveFile, multiSelect, Extensions, "Fichier base donnée sqlite")
{
    private new static IEnumerable<string> Extensions => [".sqlite"];
}