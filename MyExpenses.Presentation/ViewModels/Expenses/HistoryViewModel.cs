using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Expenses;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Systems;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Expenses;

[DirtyTracking]
public partial class HistoryViewModel : ObservableValidator
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.AccountRequired, ErrorMessage = "Account is required")]
    public partial AccountViewModel? AccountViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.DescriptionRequired, ErrorMessage = "Description is required")]
    [MaxLengthWithCode(HistoryDomain.MaxDescriptionLength, ErrorCode.DescriptionTooLong, ErrorMessage = "Description cannot exceed 255 characters")]
    public partial string? Description { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.CategoryTypeRequired, ErrorMessage = "Category type is required")]
    public partial CategoryTypeViewModel? CategoryTypeViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.ModePaymentRequired, ErrorMessage = "Mode payment is required")]
    public partial ModePaymentViewModel? ModePaymentViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.ValueRequired, ErrorMessage = "Value is required")]
    public partial double? Value { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.DateRequired, ErrorMessage = "Date is required")]
    public partial DateTime? Date { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.PlaceRequired, ErrorMessage = "Place is required")]
    public partial PlaceViewModel? PlaceViewModel { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial bool IsPointed { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial BankTransferViewModel? BankTransferFk { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial RecursiveExpenseViewModel? RecursiveExpenseViewModel { get; set; }

    public DateTime? DateAdded { get; set => SetProperty(ref field, value); }

    [DirtyTrackedProperty]
    [ObservableProperty]
    public partial DateTime? DatePointed { get; set; }
}