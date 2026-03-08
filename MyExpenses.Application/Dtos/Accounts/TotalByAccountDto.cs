namespace MyExpenses.Application.Dtos.Accounts;

public class TotalByAccountDto
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public double Total { get; init; }

    public double TotalPointed { get; init; }

    public double TotalNotPointed { get; init; }

    public required string Symbol { get; init; }
}