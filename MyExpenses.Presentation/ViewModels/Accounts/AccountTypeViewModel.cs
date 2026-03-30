using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Models.Accounts;
using Domain.Models.Validation;
using MyExpenses.Presentation.Validations.Attributes;
using MyExpenses.SharedUtils.Resources.Resx.AddEditAccount;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Accounts;

[DirtyTracking]
public partial class AccountTypeViewModel : ObservableValidator
{
    [ObservableProperty]
    public partial int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RequiredWithCode(ErrorCode.NameRequired, ErrorMessageResourceType = typeof(AddEditAccountResources), ErrorMessageResourceName = nameof(AddEditAccountResources.ButtonCancelContent))]
    [MaxLengthWithCodeAttribute(AccountTypeDomain.MaxNameLength, ErrorCode.NameTooLong, ErrorMessage = "Account type name cannot exceed 100 characters")]
    public partial string? Name { get; set; }

    public DateTime? DateAdded { get; set; }

    public IEnumerable<DomainValidationResult> GetErrorCodes()
        => GetErrors().OfType<DomainValidationResult>();
}