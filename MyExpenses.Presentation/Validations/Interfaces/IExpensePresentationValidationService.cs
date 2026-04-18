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
}