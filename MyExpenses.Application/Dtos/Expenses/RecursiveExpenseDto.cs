using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Systems;

namespace MyExpenses.Application.Dtos.Expenses;

public class RecursiveExpenseDto
{
    public int Id { get; set; }

    public AccountDto Account { get; set; }

    public string Description { get; set; }

    public string? Note { get; set; }

    public CategoryTypeDto CategoryType { get; set; }

    public ModePaymentDto ModePayment { get; set; }

    public double Value { get; set; }

    public PlaceDto Place { get; set; }

    public DateOnly StartDate { get; set; }

    public int RecursiveTotal { get; set; }

    public int? RecursiveCount { get; set; }

    public RecursiveFrequencyDto RecursiveFrequency { get; set; }

    public DateOnly NextDueDate { get; set; }

    public bool IsActive { get; set; }

    public bool ForceDeactivate { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? LastUpdated { get; set; }
}