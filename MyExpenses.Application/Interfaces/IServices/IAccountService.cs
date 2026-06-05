using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Accounts;

namespace MyExpenses.Application.Interfaces.IServices;

public interface IAccountService
{
    /// <summary>
    /// Retrieves a collection of totals by account.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="TotalByAccountDto"/> objects.</returns>
    public Task<Result<IEnumerable<TotalByAccountDto>>> GetAllTotalByAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of accounts.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="AccountDto"/> objects.</returns>
    public Task<Result<IEnumerable<AccountDto>>> GetAllAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an account by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the account to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{AccountDto}"/> object.</returns>
    public Task<Result<AccountDto>> GetAccountAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of account types.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="AccountTypeDto"/> objects.</returns>
    public Task<Result<IEnumerable<AccountTypeDto>>> GetAllAccountTypeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of currency details.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="CurrencyDto"/> objects.</returns>
    public Task<Result<IEnumerable<CurrencyDto>>> GetAllCurrencyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an account type and returns the result of the deletion operation, including any deleted items and associated details.
    /// </summary>
    /// <param name="accountTypeDto">The account type to be deleted, represented as a <see cref="AccountTypeDto"/> object.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DeletionResult"/> object with details of the deletion operation.</returns>
    public Task<DeletionResult> DeleteAccountTypeAsync(AccountTypeDto accountTypeDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new account type to the system.
    /// </summary>
    /// <param name="accountTypeDto">The account type information to be added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{AccountTypeDto}"/> indicating the outcome of the operation and the added account type details if successful.</returns>
    public Task<Result<AccountTypeDto>> CreateAccountTypeAsync(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of an account type.
    /// </summary>
    /// <param name="accountTypeDto">The account type DTO containing the new name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{AccountTypeDto}"/> indicating success or failure of the update.</returns>
    public Task<Result<AccountTypeDto>> UpdateAccountTypeName(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new currency to the system.
    /// </summary>
    /// <param name="currencyDto">The details of the currency to be added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{CurrencyDto}"/> indicating the success or failure of the operation along with the added currency details.</returns>
    public Task<Result<CurrencyDto>> CreateCurrencyAsync(CurrencyDto currencyDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the symbol for a currency.
    /// </summary>
    /// <param name="currencyDto">The currency data transfer object containing the new symbol.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{CurrencyDto}"/> indicating success or failure, and optionally the updated currency DTO.</returns>
    public Task<Result<CurrencyDto>> UpdateCurrencySymbolAsync(CurrencyDto currencyDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specified currency based on the provided currency data transfer object (DTO).
    /// </summary>
    /// <param name="currencyDto">The data transfer object representing the currency to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DeletionResult"/> indicating the outcome of the operation.</returns>
    public Task<DeletionResult> DeleteCurrencyAsync(CurrencyDto currencyDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an account and its associated data from the system.
    /// </summary>
    /// <param name="accountDto">The data transfer object representing the account to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DeletionResult"/> object indicating the success or failure of the operation, along with details of any deleted items.</returns>
    public Task<DeletionResult> DeleteAccountAsync(AccountDto accountDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing account.
    /// </summary>
    /// <param name="accountDto">The updated account data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> object with an updated <see cref="AccountDto"/>.</returns>
    public Task<Result<AccountDto>> UpdateAccountAsync(AccountDto accountDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new account using the provided account details.
    /// </summary>
    /// <param name="accountDto">The account details to be used for creating the new account.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{AccountDto}"/> indicating the success or failure of the operation and the created account details, if successful.</returns>
    public Task<Result<AccountDto>> CreateAccount(AccountDto accountDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total amounts for a specific account, including details such as total, total pointed, and total not pointed.
    /// </summary>
    /// <param name="accountDto">The account data transfer object containing the account information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="TotalByAccountDto"/> object with the total amounts for the specified account.</returns>
    public Task<TotalByAccountDto?> GetTotalByAccountAsync(AccountDto accountDto, CancellationToken cancellationToken = default);
}