using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Accounts;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Accounts;

[DirtyTracking]
public partial class CurrencyViewModel : ObservableValidator
{
    [ObservableProperty]
    public partial int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.NameRequired, ErrorMessage = "Currency name is required")]
    [MaxLengthWithCode(CurrencyDomain.MaxSymbolLength, ErrorCode.NameTooLong, ErrorMessage = "Currency symbol cannot exceed 55 characters")]
    public partial string? Symbol { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DateTime? DateAdded { get; set; }

    public IEnumerable<DomainValidationResult> GetErrorCodes()
        => GetErrors().OfType<DomainValidationResult>();
}