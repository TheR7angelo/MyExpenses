namespace MyExpenses.Presentation.ViewModels.Accounts;

public class CurrencyViewModel
{
    public int Id { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public DateTime? DateAdded { get; init; }
}