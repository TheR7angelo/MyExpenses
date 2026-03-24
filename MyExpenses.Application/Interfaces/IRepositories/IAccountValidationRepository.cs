using MyExpenses.Application.Dtos.Accounts;

namespace MyExpenses.Application.Interfaces.IRepositories;

public interface IAccountValidationRepository
{
    /// <summary>
    /// Checks if an account name already exists in the database, excluding the account with the provided ID.
    /// </summary>
    /// <param name="accountDto">
    /// The DTO that contains account details, including the name and ID for validation.
    /// </param>
    /// <param name="cancellationToken">
    /// The token to monitor for cancellation requests during asynchronous operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean indicating
    /// whether the account name already exists (true) or not (false).
    /// </returns>
    public Task<bool> IsAccountNameAlreadyExistAsync(AccountDto accountDto,
        CancellationToken cancellationToken = default);
}