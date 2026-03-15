using System.ComponentModel.DataAnnotations;
using Domain.Models.Accounts;

namespace MyExpenses.Presentation.ViewModels.Accounts;

public class AccountTypeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Account type name is required")]
    [MaxLength(AccountTypeDomain.MaxNameLength, ErrorMessage = "Account type name cannot exceed 100 characters")]
    public string? Name { get; set; }

    public DateTime? DateAdded { get; set; }
}