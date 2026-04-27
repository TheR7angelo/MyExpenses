namespace Domain.Models.Accounts;

public class AccountDomain
{
    public const int MaxNameLength = 55;

    public int Id { get; set; }

    public string Name { get; set; }

    public AccountTypeDomain AccountTypeDomain { get; set; } = new();

    public CurrencyDomain CurrencyDomain { get; set; } = new();

    public bool? Active { get; set; }

    public DateTime? DateAdded { get; set; }
}