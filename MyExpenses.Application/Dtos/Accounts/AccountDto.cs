namespace MyExpenses.Application.Dtos.Accounts;

public class AccountDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public AccountTypeDto AccountTypeDto { get; set; }

    public CurrencyDto CurrencyDto { get; set; }

    public bool Active { get; set; }

    public DateTime DateAdded { get; set; }
}