using PropertyChanged;

namespace MyExpenses.Models.Wpf.Charts;

[AddINotifyPropertyChangedInterface]
public class CategoryTotal
{
    public string? Name { get; init; }
    public string? HexadecimalColor { get; set; }
    public double? Percentage { get; set; }
    public double? Value { get; set; }
    public string? Symbol { get; set; }

    public string ValueSymbol => Value is not null ? Value + " " + Symbol : 0d + " " + Symbol;
}