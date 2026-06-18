using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Presentation.ViewModels.Analysis;

[ObservableObject]
public partial class CategoryTotalViewModel
{
    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial string? HexadecimalColor { get; set; }

    [ObservableProperty]
    public partial double? Percentage { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValueSymbol))]
    public partial double? Value { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValueSymbol))]
    public partial string? Symbol { get; set; }

    public string ValueSymbol => Value is not null ? Value + " " + Symbol : 0d + " " + Symbol;
}