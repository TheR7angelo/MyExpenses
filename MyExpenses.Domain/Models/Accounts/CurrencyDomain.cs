namespace Domain.Models.Accounts;

public class CurrencyDomain
{
    public const int MaxSymbolLength = 55;

    public int Id { get; set; }

    public string Symbol { get; set; }

    public DateTime DateAdded { get; init; }
}