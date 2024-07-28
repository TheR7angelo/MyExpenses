namespace MyExpenses.Models.IO;

public class SizeDatabase
{
    public required string FileNameWithoutExtension { get; init; }
    public required long OldSize { get; init; }
    public required long NewSize { get; init; }
}