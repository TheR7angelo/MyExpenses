using Domain.Models.Accounts;
using Domain.Models.Systems;

namespace Domain.Models.Expenses;

public class RecursiveExpenseDomain
{
    public const int MaxDescriptionLength = 255;
    public const int MaxNoteLength = 255;

    public int Id { get; set; }

    public AccountDomain Account { get; set; }

    public string Description { get; set; }

    public string? Note { get; set; }

    public CategoryTypeDomain CategoryType { get; set; }

    public ModePaymentDomain ModePayment { get; set; }

    public double Value { get; set; }

    public PlaceDomain Place { get; set; }

    public DateOnly StartDate { get; set; }

    public int RecursiveTotal { get; set; }

    public int? RecursiveCount { get; set; }

    public RecursiveFrequencyDomain RecursiveFrequency { get; set; }

    public DateOnly NextDueDate { get; set; }

    public bool IsActive { get; set; }

    public bool ForceDeactivate { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? LastUpdated { get; set; }
}