namespace MyExpenses.Models.Wpf.Charts;

public class IsSeriesTranslatable
{
    public bool IsTranslatable { get; init; }
    public bool IsGlobal { get; init; }
    public bool IsTrend { get; init; } = true;
    public string? OriginalName { get; init; }
}