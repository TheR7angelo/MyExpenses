using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation.Results;
using MyExpenses.Presentation.Validations.Validator;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Accounts;

[DirtyTracking]
public partial class AccountViewModel : BaseViewModel
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Name { get; set; }

    [DirtyTrackedProperty(DisplayMember = nameof(AccountTypeViewModel.Name))]
    [ObservableProperty]
    public partial AccountTypeViewModel? AccountTypeViewModel { get; set; }

    [DirtyTrackedProperty(DisplayMember = nameof(CurrencyViewModel.Symbol))]
    [ObservableProperty]
    public partial CurrencyViewModel? CurrencyViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial bool Active { get; set; }

    public DateTime? DateAdded { get; set; }

    public new void ValidateWithFluent(ValidationResult result)
        => base.ValidateWithFluent(result);
}