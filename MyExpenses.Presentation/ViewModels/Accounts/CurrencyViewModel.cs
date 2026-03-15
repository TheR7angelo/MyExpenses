using System.ComponentModel.DataAnnotations;
using Domain.Models.Accounts;

namespace MyExpenses.Presentation.ViewModels.Accounts;

public class CurrencyViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Currency symbol is required")]
    [MaxLength(CurrencyDomain.MaxSymbolLength, ErrorMessage = "Currency symbol cannot exceed 55 characters")]
    public string? Symbol { get; set; } = string.Empty;

    public DateTime? DateAdded { get; set; }
}