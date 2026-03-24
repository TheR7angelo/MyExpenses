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
}