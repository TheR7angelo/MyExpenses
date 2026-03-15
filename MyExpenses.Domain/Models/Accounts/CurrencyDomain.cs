namespace Domain.Models.Accounts;

public class CurrencyDomain
{
    public int Id { get; set; }

    public string Symbol { get; set; }

    public DateTime DateAdded { get; init; }
}