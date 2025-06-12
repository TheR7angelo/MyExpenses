using PropertyChanged;

namespace MyExpenses.Models.Maui.CustomPopup;

[AddINotifyPropertyChangedInterface]
public class PopupSearch
{
    public int? Id { get; init; }
    public string? Content { get; init; }
    public double? Value { get; init; }
    public bool? BValue { get; init; }
    public bool IsChecked { get; set; }
}