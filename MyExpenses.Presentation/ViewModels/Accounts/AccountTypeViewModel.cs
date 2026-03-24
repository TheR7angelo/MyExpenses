using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Accounts;

namespace MyExpenses.Presentation.ViewModels.Accounts;

public partial class AccountTypeViewModel : ObservableValidator
{
    internal string OriginalName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int Id { get; set; }

    [ObservableProperty]
    [Required(ErrorMessage = "Account type name is required")]
    [MaxLength(AccountTypeDomain.MaxNameLength, ErrorMessage = "Account type name cannot exceed 100 characters")]
    public partial string? Name { get; set; }

    public DateTime? DateAdded { get; set; }

    public bool HasNameChanged => OriginalName != Name;
}