namespace MyExpenses.Models.Ui.FilePicker;

public interface IDialog
{
    public IEnumerable<string> Extensions { get; }
    public string[]? GetFiles();
    public string? GetFile();
    public string? SaveFile();
}