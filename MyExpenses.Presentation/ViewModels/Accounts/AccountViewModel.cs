using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Accounts;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Accounts;

[DirtyTracking]
public partial class AccountViewModel : ObservableValidator
{
    [ObservableProperty]
    public partial bool IsEditing { get; set; }

    public int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.NameRequired, ErrorMessage = "Account name is required")]
    [MaxLengthWithCodeAttribute(AccountDomain.MaxNameLength, ErrorCode.NameTooLong, ErrorMessage = "Account name cannot exceed 55 characters")]
    public partial string? Name { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.AccountTypeRequired, ErrorMessage = "Account type is required")]
    public partial AccountTypeViewModel? AccountTypeViewModel { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.CurrencyRequired, ErrorMessage = "Currency is required")]
    public partial CurrencyViewModel? Currency { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.ActiveStatusRequired, ErrorMessage = "Active status is required")]
    public partial bool Active { get; set; }

    public DateTime? DateAdded { get; set; }

    public IEnumerable<DomainValidationResult> GetErrorCodes()
        => GetErrors().OfType<DomainValidationResult>();
}