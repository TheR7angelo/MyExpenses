using Domain.Models.Accounts;

namespace MyExpenses.Application.Interfaces.IRepositories;

public interface IExpenseRepository
{
    /// <summary>
    /// Retrieves the total count of all bank transactions associated with a specified account.
    /// </summary>
    /// <param name="account">The account for which to retrieve the bank transaction count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of bank transactions associated with the specified account.</returns>
    public Task<int> GetAllBankTransactionCountAsync(AccountDomain account,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total count of all recursive expenses associated with a specified account.
    /// </summary>
    /// <param name="account">The account for which to retrieve the recursive expense count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of recursive expenses associated with the specified account.</returns>
    public Task<int> GetAllRecursiveExpenseCountAsync(AccountDomain account,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the unique identifiers of all expenses associated with the specified account IDs.
    /// </summary>
    /// <param name="accountIds">An array of account IDs for which to retrieve the expense IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of integers representing the expense IDs associated with the specified accounts.</returns>
    public Task<int[]> GetAllExpenseIdAsync(int[] accountIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the unique identifiers of all bank transfers associated with the specified account IDs.
    /// </summary>
    /// <param name="accountIds">An array of account IDs for which to retrieve the bank transfer IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of integers representing the bank transfer IDs associated with the specified accounts.</returns>
    public Task<int[]> GetAllBankTransferIdsAsync(int[] accountIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the unique identifiers of all recurring transactions associated with the specified account IDs.
    /// </summary>
    /// <param name="accountIds">An array of account IDs for which to retrieve the recurring transaction IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of integers representing the recurring transaction IDs associated with the specified accounts.</returns>
    public Task<int[]> GetAllRecurringTransactionIdsAsync(int[] accountIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total count of expenses associated with a specific account asynchronously.
    /// </summary>
    /// <param name="accountDomain">The account for which the total expense count is to be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>An integer representing the count of expenses associated with the specified account.</returns>
    public Task<int> GetAllExpenseCountAsync(AccountDomain accountDomain,
        CancellationToken cancellationToken = default);
}