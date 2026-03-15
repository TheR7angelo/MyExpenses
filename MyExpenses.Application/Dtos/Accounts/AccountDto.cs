namespace MyExpenses.Application.Dtos.Accounts;

public class AccountDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public AccountTypeDto AccountType { get; set; }

    public CurrencyDto Currency { get; set; }

    public bool Active { get; set; }

    public DateTime DateAdded { get; set; }
}