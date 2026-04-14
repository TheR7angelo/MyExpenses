namespace MyExpenses.Application.Dtos.Accounts;

public class AccountTypeDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime DateAdded { get; set; }
}