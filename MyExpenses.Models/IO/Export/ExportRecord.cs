namespace MyExpenses.Models.IO.Export;

public class ExportRecord
{
    public required string Name { get; set; }
    public required Type Source { get; set; }
    public List<object?> Records { get; set; } = [];
}