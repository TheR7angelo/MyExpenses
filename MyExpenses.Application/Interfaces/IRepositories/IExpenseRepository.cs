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
}