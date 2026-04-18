namespace Domain.Models.Accounts;

public class AccountDomain
{
    public const int MaxNameLength = 55;

    public int Id { get; set; }

    public string? Name { get; set; }

    public AccountTypeDomain? AccountTypeDomain { get; set; }

    public CurrencyDomain? CurrencyDomain { get; set; }

    public bool? Active { get; set; }

    public DateTime? DateAdded { get; set; }
}