using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Accounts;

namespace MyExpenses.Presentation.ViewModels.Accounts;

public partial class AccountViewModel : ObservableValidator
{
    [ObservableProperty]
    public partial bool IsEditing { get; set; }

    internal string? OriginalName { get; set; }

    [ObservableProperty]
    public partial int Id { get; set; }

    [ObservableProperty]
    [property: Required(ErrorMessage = "Account name is required")]
    [property: MaxLength(AccountDomain.MaxNameLength, ErrorMessage = "Account name cannot exceed 55 characters")]
    public partial string? Name { get; set; }

    [ObservableProperty]
    [property: Required(ErrorMessage = "Account type is required")]
    public partial AccountTypeViewModel? AccountType { get; set; }

    [ObservableProperty]
    [property: Required(ErrorMessage = "Currency is required")]
    public partial CurrencyViewModel? Currency { get; set; }

    [ObservableProperty]
    [property: Required(ErrorMessage = "Active status is required")]
    public partial bool Active { get; set; }

    [ObservableProperty]
    [property: Required(ErrorMessage = "Date added is required")]
    public partial DateTime? DateAdded { get; set; }

    public bool HasNameChanged => OriginalName != Name;
}