using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Accounts;

namespace MyExpenses.Application.Interfaces.IServices;

public interface IAccountService
{
    /// <summary>
    /// Retrieves a collection of total amounts by account, including details such as total, total pointed, and total not pointed.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="TotalByAccountDto"/> objects.</returns>
    public Task<IEnumerable<TotalByAccountDto>> GetAllTotalByAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of account details, including properties such as name, type, currency, and activity status.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="AccountDto"/> objects.</returns>
    public Task<IEnumerable<AccountDto>> GetAllAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an account based on the specified account ID.
    /// </summary>
    /// <param name="id">The unique identifier of the account to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="AccountDto"/> object for the specified account.</returns>
    public Task<AccountDto?> GetAccountAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of account types, including details such as ID, name, and date added.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="AccountTypeDto"/> objects.</returns>
    public Task<IEnumerable<AccountTypeDto>> GetAllAccountTypeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of available currencies, including details such as ID, symbol, and date added.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="CurrencyDto"/> objects.</returns>
    public Task<IEnumerable<CurrencyDto>> GetAllCurrencyAsync(CancellationToken cancellationToken = default);

    // public Task<AccountDto> AddOrEditAsync(AccountDto accountDto, CancellationToken cancellationToken = default);

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
    public Task<Result<AccountTypeDto>> AddAccountTypeAsync(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of an account type.
    /// </summary>
    /// <param name="accountTypeDto">The data transfer object containing the updated account type information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating success or failure.</returns>
    public Task<Result> UpdateAccountTypeName(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new currency to the system.
    /// </summary>
    /// <param name="currencyDto">The details of the currency to be added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{CurrencyDto}"/> indicating the success or failure of the operation along with the added currency details.</returns>
    public Task<Result<CurrencyDto>> AddCurrencyAsync(CurrencyDto currencyDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the currency symbol for a given currency record.
    /// </summary>
    /// <param name="currencyDto">The data transfer object containing the currency details to be updated.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> object indicating the outcome of the update operation.</returns>
    public Task<Result> UpdateCurrencySymbolAsync(CurrencyDto currencyDto, CancellationToken cancellationToken = default);

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
    /// Updates the details of an existing account.
    /// </summary>
    /// <param name="accountDto">The data transfer object containing the updated account information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public Task<Result> UpdateAccountAsync(AccountDto accountDto, CancellationToken cancellationToken = default);
}