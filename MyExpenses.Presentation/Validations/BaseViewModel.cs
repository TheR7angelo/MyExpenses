using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation.Results;

namespace MyExpenses.Presentation.Validations;

/// <summary>
/// Serves as a base class for view models, providing support for validation,
/// error tracking, and notification of property changes. Inherits from
/// <see cref="CommunityToolkit.Mvvm.ComponentModel.ObservableObject"/> and
/// implements <see cref="System.ComponentModel.INotifyDataErrorInfo"/>.
/// </summary>
public abstract class BaseViewModel : ObservableObject, INotifyDataErrorInfo
{
    /// <summary>
    /// Stores the validation errors for properties in the view model.
    /// </summary>
    /// <remarks>
    /// This dictionary maps property names to a collection of associated error messages.
    /// It is used to keep track of validation errors raised during runtime and is integral
    /// to the implementation of the INotifyDataErrorInfo interface.
    /// Changes to this collection trigger the ErrorsChanged event to notify subscribers of updates
    /// to the validation state for one or more properties.
    /// </remarks>
    private readonly Dictionary<string, List<string>> _errors = new();

    /// <summary>
    /// Occurs when the validation errors for a property have changed.
    /// </summary>
    /// <remarks>
    /// This event is raised whenever errors related to a specific property
    /// of the object are added, removed, or modified. Subscribers can listen
    /// to this event to respond to changes in validation state, such as updating
    /// UI elements or triggering other actions based on property error conditions.
    /// </remarks>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Gets a value indicating whether the object has validation errors.
    /// </summary>
    /// <remarks>
    /// The property evaluates to <c>true</c> if there are any validation errors present,
    /// otherwise it returns <c>false</c>. Validation errors are managed internally within
    /// the class by tracking them in a dictionary. The property is commonly used to
    /// determine if the current state of the object is valid or requires correction.
    /// </remarks>
    public bool HasErrors => _errors.Count > 0;

    /// <summary>
    /// Retrieves the validation errors for the specified property.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property for which to retrieve validation errors. If null or empty,
    /// errors for all properties are included.
    /// </param>
    /// <returns>
    /// A collection of error messages associated with the specified property, or an empty
    /// collection if no errors exist.
    /// </returns>
    public IEnumerable GetErrors(string? propertyName)
        => _errors.GetValueOrDefault(propertyName ?? "") ?? Enumerable.Empty<string>();

    /// <summary>
    /// Validates the object using the provided FluentValidation results and updates
    /// the error collection and notification system accordingly.
    /// </summary>
    /// <param name="result">The validation result containing details of validation errors.</param>
    protected void ValidateWithFluent(ValidationResult result)
    {
        var propertiesToNotify = _errors.Keys
            .Concat(result.Errors.Select(e => e.PropertyName))
            .ToHashSet();

        _errors.Clear();

        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                if (!_errors.TryGetValue(error.PropertyName, out var list))
                    _errors[error.PropertyName] = list = [];

                list.Add(error.ErrorMessage);
            }
        }

        foreach (var propertyName in propertiesToNotify)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        OnPropertyChanged(nameof(HasErrors));
    }
}