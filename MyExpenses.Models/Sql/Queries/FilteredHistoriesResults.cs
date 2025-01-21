using MyExpenses.Models.Sql.Bases.Views;

namespace MyExpenses.Models.Sql.Queries;

public struct FilteredHistoriesResults
{
    public IEnumerable<VHistory> Histories { get; init; }
    public int TotalRowCount { get; init; }
    public int TotalFilteredRowCount { get; init; }
}