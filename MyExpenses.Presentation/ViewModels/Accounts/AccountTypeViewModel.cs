using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation.Results;
using TheR7angelo.DirtyTracking.Abstractions;

namespace MyExpenses.Presentation.ViewModels.Accounts;

[DirtyTracking]
public partial class AccountTypeViewModel : ObservableObject, INotifyDataErrorInfo
{
    [ObservableProperty]
    public partial int Id { get; set; }

    [DirtyTrackedProperty]
    [ObservableProperty]
    // [RequiredWithCode(ErrorCode.NameRequired, ErrorMessageResourceType = typeof(AddEditAccountResources), ErrorMessageResourceName = nameof(AddEditAccountResources.ButtonCancelContent))]
    // [MaxLengthWithCodeAttribute(AccountTypeDomain.MaxNameLength, ErrorCode.NameTooLong, ErrorMessage = "Account type name cannot exceed 100 characters")]
    public partial string? Name { get; set; }

    public DateTime? DateAdded { get; set; }

    // TODO try
    private readonly Dictionary<string, List<string>> _errors = new();

    // TODO try
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    // TODO try
    public IEnumerable GetErrors(string? propertyName)
        => _errors.GetValueOrDefault(propertyName ?? "") ?? Enumerable.Empty<string>();

    // TODO try
    public bool HasErrors => _errors.Any();

    // TODO try
    public void ValidateWithFluent(ValidationResult result)
    {
        _errors.Clear();
        foreach (var error in result.Errors)
        {
            if (!_errors.ContainsKey(error.PropertyName)) _errors[error.PropertyName] = new();
            _errors[error.PropertyName].Add(error.ErrorMessage);
        }

        // On prévient WPF de rafraîchir l'affichage
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null));
    }
}