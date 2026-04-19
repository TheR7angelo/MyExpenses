using Domain.Models.Accounts;
using Domain.Models.Categories;
using Domain.Models.Expenses;
using Domain.Models.Validation;

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
    /// Retrieves an array of all expense IDs associated with a specified category type.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type for which to retrieve the expense IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of expense IDs associated with the specified category type.</returns>
    public Task<int[]> GetAllExpenseIdAsync(CategoryTypeDomain categoryTypeDomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the unique identifiers of all bank transfers associated with the specified account IDs.
    /// </summary>
    /// <param name="accountIds">An array of account IDs for which to retrieve the bank transfer IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of integers representing the bank transfer IDs associated with the specified accounts.</returns>
    public Task<int[]> GetAllBankTransferIdsAsync(int[] accountIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the IDs of all bank transfers associated with a specified category type domain.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type domain for which to retrieve the bank transfer IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of IDs representing the bank transfers associated with the specified category type domain.</returns>
    public Task<int[]> GetAllBankTransferIdsAsync(CategoryTypeDomain categoryTypeDomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the unique identifiers of all recurring transactions associated with the specified account IDs.
    /// </summary>
    /// <param name="accountIds">An array of account IDs for which to retrieve the recurring transaction IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of integers representing the recurring transaction IDs associated with the specified accounts.</returns>
    public Task<int[]> GetAllRecurringTransactionIdsAsync(int[] accountIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the IDs of all recurring transactions associated with the specified category type.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type associated with the recurring transactions.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of IDs corresponding to recurring transactions associated with the specified category type.</returns>
    public Task<int[]> GetAllRecurringTransactionIdsAsync(CategoryTypeDomain categoryTypeDomain,
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
    /// Retrieves the total count of all expenses associated with a specific category type.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type domain for which to retrieve the expense count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of expenses associated with the specified category type.</returns>
    public Task<int> GetAllExpenseCountAsync(CategoryTypeDomain categoryTypeDomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total count of all bank transactions associated with a specified category type.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type for which to retrieve the bank transaction count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of bank transactions associated with the specified category type.</returns>
    public Task<int> GetAllBankTransactionCountAsync(CategoryTypeDomain categoryTypeDomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the count of all recursive expenses associated with the specified category type.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type for which to retrieve the recursive expense count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of recursive expenses associated with the specified category type.</returns>
    public Task<int> GetAllRecursiveExpenseCountAsync(CategoryTypeDomain categoryTypeDomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all category types from the data source.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of category types.</returns>
    public Task<IEnumerable<CategoryTypeDomain>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new category type asynchronously.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type domain object containing details of the category to add.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="Result"/> object indicating the success or failure of the operation.</returns>
    public Task<Result> AddCategoryTypeAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified account type and returns the result of the deletion process.
    /// </summary>
    /// <param name="accountType">The account type to be deleted.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The result of the deletion operation, including details of any deleted items.</returns>
    public Task<DeletionResult> DeleteCategoryTypeAsync(CategoryTypeDomain accountType,
        CancellationToken cancellationToken = default);
}