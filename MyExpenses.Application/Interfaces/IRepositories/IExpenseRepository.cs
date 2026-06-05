using Domain.Models.Accounts;
using Domain.Models.Expenses;
using Domain.Models.Systems;
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
    /// Retrieves the unique identifiers of all expenses associated with the specified account IDs.
    /// </summary>
    /// <param name="accountIds">An array of account IDs for which to retrieve the expense IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of integers representing the expense IDs associated with the specified accounts.</returns>
    public Task<int[]> GetAllExpenseIdAsync(int[] accountIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the IDs of all expenses associated with the specified accounts.
    /// </summary>
    /// <param name="accountDomain">The account domain for which to retrieve the associated expense IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of expense IDs associated with the specified accounts.</returns>
    public Task<int[]> GetAllExpenseIdAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an array of all expense IDs associated with a specified category type.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type for which to retrieve the expense IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of expense IDs associated with the specified category type.</returns>
    public Task<int[]> GetAllExpenseIdAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an array of all expense IDs associated with a specified place.
    /// </summary>
    /// <param name="placeDomain">The place for which to retrieve the expense IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of expense IDs associated with the specified place.</returns>
    public Task<Result<int[]>> GetAllExpenseIdAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default);

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
    public Task<int[]> GetAllBankTransferIdsAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the unique identifiers of all bank transfers associated with the specified payment method.
    /// </summary>
    /// <param name="modePaymentDomain">The payment method domain object containing details about the payment method.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>A result containing an array of integers representing the bank transfer IDs associated with the specified payment method.</returns>
    public Task<Result<int[]>> GetAllBankTransferIdsAsync(ModePaymentDomain modePaymentDomain, CancellationToken cancellationToken = default);

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
    public Task<int[]> GetAllRecurringTransactionIdsAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the IDs of all recurring transactions associated with the specified account.
    /// </summary>
    /// <param name="accountDomain">The account domain from which the recurring transaction IDs will be retrieved.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of IDs representing the recurring transactions associated with the specified account.</returns>
    public Task<int[]> GetAllRecurringTransactionIdsAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the IDs of all recurring transactions associated with the specified place.
    /// </summary>
    /// <param name="placeDomain">The place domain from which the recurring transaction IDs will be retrieved.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of IDs representing the recurring transactions associated with the specified place.</returns>
    public Task<Result<int[]>> GetAllRecurringTransactionIdsAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total count of expenses associated with a specific account asynchronously.
    /// </summary>
    /// <param name="accountDomain">The account for which the total expense count is to be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>An integer representing the count of expenses associated with the specified account.</returns>
    public Task<int> GetAllExpenseCountAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total count of all expenses associated with a specific category type.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type domain for which to retrieve the expense count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of expenses associated with the specified category type.</returns>
    public Task<int> GetAllExpenseCountAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total count of all expenses associated with a specified place.
    /// </summary>
    /// <param name="placeDomain">The domain model representing the place for which to retrieve the expense count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of expenses associated with the specified place.</returns>
    public Task<int> GetAllExpenseCountAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total count of all bank transactions associated with a specified category type.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type for which to retrieve the bank transaction count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of bank transactions associated with the specified category type.</returns>
    public Task<int> GetAllBankTransactionCountAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total count of all bank transactions associated with a specified place.
    /// </summary>
    /// <param name="placeDomain">The place for which to retrieve the bank transaction count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of bank transactions associated with the specified place.</returns>
    public Task<int> GetAllBankTransactionCountAsync(PlaceDomain placeDomain, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the IDs of all bank transfers associated with the specified accounts.
    /// </summary>
    /// <param name="accountDomain">The account for which to retrieve the bank transfer IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of bank transfer IDs associated with the specified accounts.</returns>
    public Task<int[]> GetAllBankTransferIdsAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the IDs of all bank transfers associated with the specified places.
    /// </summary>
    /// <param name="placeDomain">The place for which to retrieve the bank transfer IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of bank transfer IDs associated with the specified places.</returns>
    public Task<Result<int[]>> GetAllBankTransferIdsAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total count of all recursive expenses associated with a specified account.
    /// </summary>
    /// <param name="account">The account for which to retrieve the recursive expense count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of recursive expenses associated with the specified account.</returns>
    public Task<int> GetAllRecursiveExpenseCountAsync(AccountDomain account, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the count of all recursive expenses associated with the specified category type.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type for which to retrieve the recursive expense count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of recursive expenses associated with the specified category type.</returns>
    public Task<int> GetAllRecursiveExpenseCountAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the count of all recursive expenses associated with the specified place.
    /// </summary>
    /// <param name="placeDomain">The place for which to retrieve the recursive expense count.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The total number of recursive expenses associated with the specified place.</returns>
    public Task<int> GetAllRecursiveExpenseCountAsync(PlaceDomain placeDomain, CancellationToken cancellationToken = default);

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
    /// <returns>A <see cref="Result{CategoryTypeDomain}"/> object indicating the success or failure of the operation.</returns>
    public Task<Result<CategoryTypeDomain>> CreateCategoryTypeAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified account type and returns the result of the deletion process.
    /// </summary>
    /// <param name="accountType">The account type to be deleted.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The result of the deletion operation, including details of any deleted items.</returns>
    public Task<DeletionResult> DeleteCategoryTypeAsync(CategoryTypeDomain accountType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of an existing category type in the system.
    /// </summary>
    /// <param name="categoryType">The category type domain object containing the updated name and related information.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>A result indicating whether the update was successful, including any relevant error information if applicable.</returns>
    public Task<Result> UpdateCategoryTypeNameAsync(CategoryTypeDomain categoryType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new expense record based on the provided domain model.
    /// </summary>
    /// <param name="historyDomain">The domain model representing the expense to be created.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>A result containing the created expense record.</returns>
    public Task<Result<HistoryDomain>> CreateExpenseAsync(HistoryDomain historyDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all available modes of payment from the data source.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="ModePaymentDomain"/> objects representing all modes of payment.</returns>
    public Task<IEnumerable<ModePaymentDomain>> GetAllModePaymentAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the mode of payment details associated with a specified ID.
    /// </summary>
    /// <param name="modePaymentId">The unique identifier of the mode of payment to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>The mode of payment details associated with the specified ID, or null if not found.</returns>
    public Task<ModePaymentDomain?> GetModePaymentByIdAsync(int modePaymentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new bank transfer and the associated historical records in the system.
    /// </summary>
    /// <param name="bankTransferDomain">The bank transfer domain object containing the details of the transfer.</param>
    /// <param name="historyDomain">The history domain object containing the details of the associated historical records.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>A result containing the created bank transfer object and a collection of associated historical records.</returns>
    public Task<Result<(BankTransferDomain bankTransfer, IEnumerable<HistoryDomain> historiesDomain)>>
        CreateBankTransferAsync(BankTransferDomain bankTransferDomain, HistoryDomain historyDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all category types associated with a specified color.
    /// </summary>
    /// <param name="colorDomain">The color domain object used to filter category types.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>A result containing an enumerable of category type domain objects associated with the specified color.</returns>
    public Task<Result<IEnumerable<CategoryTypeDomain>>> GetAllByColorAsync(ColorDomain colorDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all expense IDs associated with the provided category type domains.
    /// </summary>
    /// <param name="categoryTypeDomain">An array of category type domains for which to fetch the expense IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of expense IDs associated with the specified category type domains.</returns>
    public Task<Result<int[]>> GetAllExpenseIdAsync(CategoryTypeDomain[] categoryTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the identifiers of all bank transfer transactions associated with the specified category type domains.
    /// </summary>
    /// <param name="categoryTypeDomain">An array of category type domains for which to retrieve the bank transfer transaction identifiers.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>An array of identifiers for the bank transfer transactions associated with the specified category type domains.</returns>
    public Task<Result<int[]>> GetAllBankTransferIdsAsync(CategoryTypeDomain[] categoryTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the IDs of all recurring transactions associated with the specified category types.
    /// </summary>
    /// <param name="categoryTypeDomain">The category types for which to retrieve the recurring transaction IDs.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation, if needed.</param>
    /// <returns>A result object containing the IDs of all recurring transactions associated with the specified category types.</returns>
    public Task<Result<int[]>> GetAllRecurringTransactionIdsAsync(CategoryTypeDomain[] categoryTypeDomain, CancellationToken cancellationToken = default);
}