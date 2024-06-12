namespace MyExpenses.Models.IO;

public class ExistingDatabase
{
    public string? FilePath { get; init; }
    public string? FileName => Path.GetFileName(FilePath);
    public string? FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FilePath);
}