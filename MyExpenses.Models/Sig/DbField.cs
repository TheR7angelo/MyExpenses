namespace MyExpenses.Models.Sig;

public class DbField
{
    public required string Name { get; init; }
    public required Type Type { get; init; }
    public int? MaxLength { get; set; }
    public int? MaxPrecision { get; set; }
}