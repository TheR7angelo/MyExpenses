namespace MyExpenses.Application.Interfaces.IRepositories;

public interface IExpenseValidationRepository
{
    /// <summary>
    /// Checks if a category type name already exists in the database.
    /// </summary>
    /// <param name="categoryTypeName">
    /// The category type name to be checked for existence.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests during the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the category type name already exists (true) or not (false).
    /// </returns>
    public Task<bool> IsCategoryTypeNameAlreadyExistAsync(string categoryTypeName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category type name already exists in the database for a specified category type ID.
    /// </summary>
    /// <param name="categoryTypeName">
    /// The category type name to be checked for existence.
    /// </param>
    /// <param name="id">
    /// The unique identifier of the category type to be associated with the validation.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests during the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the category type name already exists for the specified ID (true) or not (false).
    /// </returns>
    public Task<bool> IsCategoryTypeNameAlreadyExistAsync(string categoryTypeName, int id,
        CancellationToken cancellationToken = default);
}