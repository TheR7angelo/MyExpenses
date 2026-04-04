using Domain.Models.Accounts;
using Domain.Models.Categories;

namespace Domain.Models.Expenses;

public class RecursiveExpenseDomain
{
    public const int MaxDescriptionLength = 255;
    public const int MaxNoteLength = 255;

    public int Id { get; set; }

    public required AccountDomain Account { get; set; }

    public required string Description { get; set; }

    public string? Note { get; set; }

    public required CategoryTypeDomain CategoryType { get; set; }

    public required ModePaymentDomain ModePayment { get; set; }

    public double Value { get; set; }

    public required PlaceDomain Place { get; set; }

    public required DateOnly StartDate { get; set; }

    public int? RecursiveTotal { get; set; }

    public int RecursiveCount { get; set; }

    public required RecursiveFrequencyDomain FrequencyDomain { get; set; }

    public DateOnly NextDueDate { get; set; }

    public bool IsActive { get; set; }

    public bool ForceDeactivate { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? LastUpdated { get; set; }

    public HistoryDomain[] History { get; set; } = [];
}