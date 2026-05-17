using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation.Results;
using MyExpenses.Presentation.Validations.Validator;
using MyExpenses.Presentation.ViewModels.Accounts;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Expenses;

[DirtyTracking]
public partial class BankTransferViewModel : BaseViewModel
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial double? Value { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial AccountViewModel? FromAccount { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial AccountViewModel? ToAccount { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? MainReason { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? AdditionalReason { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial DateTime? Date { get; set; }

    public DateTime? DateAdded { get; set; }

    public new void ValidateWithFluent(ValidationResult result)
        => base.ValidateWithFluent(result);
}