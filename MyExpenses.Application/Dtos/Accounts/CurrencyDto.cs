namespace MyExpenses.Application.Dtos.Accounts;

public class CurrencyDto
{
    public int Id { get; set; }

    public string? Symbol { get; set; }

    public DateTime? DateAdded { get; init; }
}