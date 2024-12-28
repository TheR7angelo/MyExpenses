namespace MyExpenses.Models.Wpf.Charts;

public class CategoryTotal
{
    public string? Name { get; init; }
    public string? HexadecimalColor { get; init; }
    public double? Percentage { get; init; }
    public double? Value { get; init; }
    public string? Symbol { get; init; }

    public string ValueSymbol => Value is not null ? Value + " " + Symbol : 0d + " " + Symbol;
}