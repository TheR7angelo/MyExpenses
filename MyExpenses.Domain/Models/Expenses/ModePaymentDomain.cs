namespace Domain.Models.Expenses;

public class ModePaymentDomain
{
    public const int MaxNameLength = 55;

    public int Id { get; set; }

    public string? Name { get; set; }

    public bool CanBeDeleted { get; init; }

    public DateTime DateAdded { get; init; } = DateTime.Now;

    public HistoryDomain[] Histories { get; set; } = [];

    public RecursiveExpenseDomain[] RecursiveExpenses { get; set; } = [];
}