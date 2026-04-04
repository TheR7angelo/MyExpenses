using Domain.Models.Accounts;

namespace Domain.Models.Expenses;

public class BankTransferDomain
{
    public const int MaxMainReasonLength = 100;
    public const int MaxAdditionalReasonLength = 255;

    public int Id { get; set; }

    public double Value { get; set; }

    public required AccountDomain FromAccount { get; set; }

    public required AccountDomain ToAccount { get; set; }

    public required string MainReason { get; set; }

    public string? AdditionalReason { get; set; }

    public DateTime Date { get; set; }

    public DateTime DateAdded { get; set; }

    public HistoryDomain[] Histories { get; set; } = [];
}