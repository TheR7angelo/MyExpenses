using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Systems;

namespace MyExpenses.Application.Dtos.Expenses;

public class HistoryDto
{
   public int Id { get; set; }

    public required AccountDto Account { get; set; }

    public required string Description { get; set; }

    public required CategoryTypeDto CategoryType { get; set; }

    public required ModePaymentDto ModePayment { get; set; }

    public double Value { get; set; }

    public DateTime Date { get; set; }

    public required PlaceDto Place { get; set; }

    public bool IsPointed { get; set; }

    public BankTransferDto? BankTransfer { get; init; }

    public RecursiveExpenseDto? RecursiveExpense { get; init; }

    public DateTime DateAdded { get; set; }

    public DateTime? DatePointed { get; set; }
}