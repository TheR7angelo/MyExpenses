using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation.Results;
using MyExpenses.Presentation.Validations;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Accounts;

[DirtyTracking]
public partial class AccountViewModel : BaseViewModel
{
    [ObservableProperty]
    public partial bool IsEditing { get; set; }

    public int Id { get; set; }

    [ObservableProperty]
    public partial string? Name { get; set; }

    [ObservableProperty]
    public partial AccountTypeViewModel? AccountTypeViewModel { get; set; }

    [ObservableProperty]
    public partial CurrencyViewModel? CurrencyViewModel { get; set; }

    [ObservableProperty]
    public partial bool Active { get; set; }

    public DateTime? DateAdded { get; set; }

    public new void ValidateWithFluent(ValidationResult result)
        => base.ValidateWithFluent(result);
}