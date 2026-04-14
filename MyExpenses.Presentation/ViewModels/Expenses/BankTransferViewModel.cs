using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Expenses;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using MyExpenses.Presentation.ViewModels.Accounts;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Expenses;

[DirtyTracking]
public partial class BankTransferViewModel : ObservableValidator
{
    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.ValueRequired, ErrorMessage = "Value is required")]
    public partial double? Value { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.AccountRequired, ErrorMessage = "Account from is required")]
    public partial AccountViewModel? FromAccount { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.AccountRequired, ErrorMessage = "Account to is required")]
    public partial AccountViewModel? ToAccount { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.DescriptionRequired, ErrorMessage = "Main reason is required")]
    [MaxLengthWithCode(BankTransferDomain.MaxMainReasonLength, ErrorCode.DescriptionTooLong, ErrorMessage = "Main reason cannot exceed 100 characters")]
    public partial string? MainReason { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [MaxLengthWithCode(BankTransferDomain.MaxAdditionalReasonLength, ErrorCode.DescriptionTooLong, ErrorMessage = "Additional reason cannot exceed 255 characters")]
    public partial string? AdditionalReason { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.DateRequired, ErrorMessage = "Date is required")]
    public partial DateTime? Date { get; set; }

    public DateTime? DateAdded { get; set; }
}