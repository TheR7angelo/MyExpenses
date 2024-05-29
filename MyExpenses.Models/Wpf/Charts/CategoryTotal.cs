namespace MyExpenses.Models.Wpf.Charts;

public class CategoryTotal
{
    public string? Name { get; set; }
    public string? HexadecimalColor { get; set; }
    public double? Percentage { get; set; }
    public double? Value { get; set; }
    public string? Symbol { get; set; }

    public string ValueSymbol => Value is not null ? Value + " " + Symbol : 0d + " " + Symbol;
}