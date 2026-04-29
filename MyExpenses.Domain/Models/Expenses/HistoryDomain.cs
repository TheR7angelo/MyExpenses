using Domain.Models.Accounts;
using Domain.Models.Systems;

namespace Domain.Models.Expenses;

public class HistoryDomain
{
    public const int MaxDescriptionLength = 255;

    public int Id { get; set; }

    public AccountDomain Account { get; set; }

    public string Description { get; set; }

    public CategoryTypeDomain CategoryType { get; set; }

    public ModePaymentDomain ModePayment { get; set; }

    public double Value { get; set; }

    public DateTime Date { get; set; }

    public PlaceDomain Place { get; set; }

    public bool IsPointed { get; set; }

    public BankTransferDomain? BankTransfer { get; init; }

    public RecursiveExpenseDomain? RecursiveExpense { get; init; }

    public DateTime DateAdded { get; set; }

    public DateTime? DatePointed { get; set; }
}