using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation.Results;
using MyExpenses.Presentation.Validations.Validator;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Systems;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Expenses;

[DirtyTracking]
public partial class HistoryViewModel : BaseViewModel
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial AccountViewModel? AccountViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial string? Description { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial CategoryTypeViewModel? CategoryTypeViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial ModePaymentViewModel? ModePaymentViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial double? Value { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial DateTime? Date { get; set; } = DateTime.Now;

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial PlaceViewModel? PlaceViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial bool IsPointed { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial BankTransferViewModel? BankTransferViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial RecursiveExpenseViewModel? RecursiveExpenseViewModel { get; set; }

    public DateTime? DateAdded { get; set; } = DateTime.Now;

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial DateTime? DatePointed { get; set; }

    public new void ValidateWithFluent(ValidationResult result)
        => base.ValidateWithFluent(result);
}