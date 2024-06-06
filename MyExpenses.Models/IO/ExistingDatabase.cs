namespace MyExpenses.Models.IO;

public class ExistingDatabase
{
    public string? FilePath { get; init; }
    public string? FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FilePath);
}