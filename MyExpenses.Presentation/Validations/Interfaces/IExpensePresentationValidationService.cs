using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Validations.Interfaces;

public interface IExpensePresentationValidationService
{
    /// <summary>
    /// Checks if the provided category type name is available for use.
    /// </summary>
    /// <param name="input">The category type name to check for availability.</param>
    /// <param name="cancellationToken">A cancellation token to observe while awaiting the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating whether the category type name is available.</returns>
    public Task<bool> IsCategoryTypeNameAvailableAsync(string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the provided category type name is available for use based on the specified input and category type details.
    /// </summary>
    /// <param name="input">The category type name to check for availability.</param>
    /// <param name="categoryTypeViewModel">The view model containing details of the category type to validate against.</param>
    /// <param name="cancellationToken">A cancellation token to observe while awaiting the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating whether the category type name is available.</returns>
    public Task<bool> IsCategoryTypeNameAvailableAsync(string input, CategoryTypeViewModel categoryTypeViewModel,
        CancellationToken cancellationToken = default);
}