namespace MyExpenses.Application.Dtos.Accounts;

public class TotalByAccountDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public double Total { get; init; }

    public double TotalPointed { get; init; }

    public double TotalNotPointed { get; init; }

    public string Symbol { get; init; } = string.Empty;
}