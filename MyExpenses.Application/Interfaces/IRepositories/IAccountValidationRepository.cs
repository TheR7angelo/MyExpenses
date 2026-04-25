namespace MyExpenses.Application.Interfaces.IRepositories;

public interface IAccountValidationRepository
{
    /// <summary>
    /// Checks if an account name already exists in the database.
    /// </summary>
    /// <param name="accountName">
    /// The name of the account to validate for uniqueness.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests during the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the account name already exists (true) or not (false).
    /// </returns>
    public Task<bool> IsAccountNameAlreadyExistAsync(string accountName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an account type name already exists in the database, excluding the account type with the provided ID.
    /// </summary>
    /// <param name="accountTypeName">
    /// The name of the account type to validate for uniqueness.
    /// </param>
    /// <param name="id">
    /// The ID of the account type to exclude from the check.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests during the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the account type name already exists (true) or not (false).
    /// </returns>
    public Task<bool> IsAccountTypeNameAlreadyExistAsync(string accountTypeName, int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an account type name already exists in the system.
    /// </summary>
    /// <param name="accountTypeName">
    /// The name of the account type to validate for uniqueness.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to signal cancellation of the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a boolean indicating
    /// whether the account type name already exists (true) or not (false).
    /// </returns>
    public Task<bool> IsAccountTypeNameAlreadyExistAsync(string accountTypeName,
        CancellationToken cancellationToken = default);
}