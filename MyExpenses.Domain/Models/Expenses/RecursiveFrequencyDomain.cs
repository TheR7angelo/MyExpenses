namespace Domain.Models.Expenses;

public class RecursiveFrequencyDomain
{
    public const int MaxFrequencyLength = 55;
    public const int MaxDescriptionLength = 100;

    public int Id { get; set; }

    public required string Frequency { get; set; }

    public required string Description { get; set; }

    public RecursiveExpenseDomain[] RecursiveExpenses { get; set; } = [];
}