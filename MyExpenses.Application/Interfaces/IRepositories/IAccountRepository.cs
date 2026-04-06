using Domain.Models.Accounts;
using Domain.Models.Validation;

namespace MyExpenses.Application.Interfaces.IRepositories;

public interface IAccountRepository
{
    /// <summary>
    /// Retrieves the total amounts for all accounts asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A collection of <see cref="TotalByAccountDomain"/> objects containing account totals and related information.</returns>
    public Task<IEnumerable<TotalByAccountDomain>> GetTotalByAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the names of all accounts asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A collection of strings representing the names of the available accounts.</returns>
    public Task<IEnumerable<string>> GetAllAccountNames(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all accounts asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A collection of <see cref="AccountDomain"/> objects representing the available accounts.</returns>
    public Task<IEnumerable<AccountDomain>> GetAllAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all accounts asynchronously based on the specified account type.
    /// </summary>
    /// <param name="accountType">The type of account to filter the retrieval process.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>An <see cref="AccountDomain"/> object containing details of the retrieved accounts.</returns>
    public Task<IEnumerable<AccountDomain>> GetAllAccountAsync(AccountTypeDomain accountType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all account types asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A collection of <see cref="AccountTypeDomain"/> objects representing the available account types.</returns>
    public Task<IEnumerable<AccountTypeDomain>> GetAllAccountTypeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all available currencies asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A collection of <see cref="CurrencyDomain"/> objects representing the available currencies.</returns>
    public Task<IEnumerable<CurrencyDomain>> GetAllCurrencyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified account type asynchronously.
    /// </summary>
    /// <param name="accountType">The <see cref="AccountTypeDomain"/> object representing the account type to be deleted.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="DeletionResult"/> object representing the result of the deletion operation, including details about deleted items.</returns>
    public Task<DeletionResult> DeleteAccountTypeAsync(AccountTypeDomain accountType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total count of expenses associated with a specific account asynchronously.
    /// </summary>
    /// <param name="accountDomain">The account for which the total expense count is to be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>An integer representing the count of expenses associated with the specified account.</returns>
    public Task<int> GetAllExpenseCountAsync(AccountDomain accountDomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new account type asynchronously.
    /// </summary>
    /// <param name="accountTypeDomain">The account type to be added.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public Task<Result> AddAccountTypeAsync(AccountTypeDomain accountTypeDomain,
        CancellationToken cancellationToken = default);
}