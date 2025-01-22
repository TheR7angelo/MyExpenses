using MyExpenses.Models.Sql.Bases.Views;

namespace MyExpenses.Models.Sql.Queries;

public struct FilteredBankTransfersResults
{
    public IEnumerable<VBankTransferSummary> BankTransferSummaries { get; init; }
    public int TotalRowCount { get; init; }
    public int TotalFilteredRowCount { get; init; }
}