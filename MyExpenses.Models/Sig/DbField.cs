namespace MyExpenses.Models.Sig;

public class DbField
{
    public required string Name { get; set; }
    public required Type Type { get; set; }
    public int? MaxLength { get; set; }
    public int? MaxPrecision { get; set; }
}