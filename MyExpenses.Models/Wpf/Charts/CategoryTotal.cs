using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Models.Wpf.Charts;

[ObservableObject]
public partial class CategoryTotal
{
    public string? Name { get; init => SetProperty(ref field, value); }
    public string? HexadecimalColor { get; set => SetProperty(ref field, value); }
    public double? Percentage { get; set => SetProperty(ref field, value); }
    public double? Value { get; set => SetProperty(ref field, value); }
    public string? Symbol { get; set => SetProperty(ref field, value); }

    public string ValueSymbol => Value is not null ? Value + " " + Symbol : 0d + " " + Symbol;
}