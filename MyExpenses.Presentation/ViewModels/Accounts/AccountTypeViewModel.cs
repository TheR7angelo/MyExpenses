namespace MyExpenses.Presentation.ViewModels.Accounts;

public class AccountTypeViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime? DateAdded { get; set; }
}