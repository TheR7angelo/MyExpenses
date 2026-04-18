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
    /// Adds a new account type to the system using the provided account type details.
    /// </summary>
    /// <param name="accountTypeDto">An object containing the details of the account type to be added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> object indicating the success or failure of the operation.</returns>
    public Task<Result> AddAccountTypeAsync(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of an account type.
    /// </summary>
    /// <param name="accountTypeDto">The data transfer object containing the updated account type information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating success or failure.</returns>
    public Task<Result> UpdateAccountTypeName(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default);
}