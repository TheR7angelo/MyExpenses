namespace MyExpenses.Models.Maui.CustomPopup;

public class PopupSearch
{
    public required int Id { get; init; }
    public required string Content { get; init; }
    public required bool IsChecked { get; set; }
}