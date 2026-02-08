using CommunityToolkit.Mvvm.ComponentModel;

namespace MyExpenses.Models.Maui.CustomPopup;

public partial class PopupSearch : ObservableObject
{
    public int? Id { get; init; }
    public string? Content { get; init; }
    public double? Value { get; init; }
    public bool? BValue { get; init; }

    public bool IsChecked
    {
        get;
        set => SetProperty(ref field, value);
    }
}