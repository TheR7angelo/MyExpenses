using MyExpenses.Application.Dtos.Accounts;

namespace MyExpenses.Application.Dtos.Expenses;

public class BankTransferDto
{
    public int Id { get; set; }

    public double Value { get; set; }

    public AccountDto FromAccount { get; set; }

    public AccountDto ToAccount { get; set; }

    public string MainReason { get; set; }

    public string? AdditionalReason { get; set; }

    public DateTime Date { get; set; }

    public DateTime DateAdded { get; set; }
}