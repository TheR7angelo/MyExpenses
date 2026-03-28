using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Accounts;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Accounts;

[DirtyTracking]
public partial class AccountTypeViewModel : ObservableValidator
{
    [ObservableProperty]
    public partial int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [Required(ErrorMessage = "Account type name is required")]
    [MaxLength(AccountTypeDomain.MaxNameLength, ErrorMessage = "Account type name cannot exceed 100 characters")]
    public partial string? Name { get; set; }

    public DateTime? DateAdded { get; set; }
}