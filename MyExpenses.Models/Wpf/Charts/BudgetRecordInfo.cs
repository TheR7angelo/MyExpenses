namespace MyExpenses.Models.Wpf.Charts;

public class BudgetRecordInfo
{
    public required double Value { get; init; }
    public required string Status { get; init; }
    public required double DifferenceValue { get; init; }
    public required double Percentage { get; init; }
}