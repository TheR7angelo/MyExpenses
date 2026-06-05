using Domain.Models.Accounts;
using Domain.Models.Validation;

namespace MyExpenses.Application.Interfaces.IRepositories;

public interface IAccountRepository
{
    /// <summary>
    /// Retrieves a collection of total amounts by account asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A result containing either a collection of <see cref="TotalByAccountDomain"/> objects or an error.</returns>
    public Task<Result<IEnumerable<TotalByAccountDomain>>> GetAllTotalByAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of all accounts asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A result containing either a collection of <see cref="AccountDomain"/> objects or an error.</returns>
    public Task<Result<IEnumerable<AccountDomain>>> GetAllAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of accounts by type asynchronously.
    /// </summary>
    /// <param name="accountType">The type of account to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A result containing either a collection of <see cref="AccountDomain"/> objects or an error.</returns>
    public Task<Result<IEnumerable<AccountDomain>>> GetAllAccountAsync(AccountTypeDomain accountType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all accounts asynchronously based on the provided currency.
    /// </summary>
    /// <param name="currencyDomain">The currency domain to filter accounts by.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A result containing either a collection of <see cref="AccountDomain"/> objects or an error.</returns>
    public Task<Result<IEnumerable<AccountDomain>>> GetAllAccountAsync(CurrencyDomain currencyDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an account by its identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the account to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A result containing either the retrieved <see cref="AccountDomain"/> object or an error.</returns>
    public Task<Result<AccountDomain>> GetAccountAsync(int id, CancellationToken cancellationToken = default);

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
    /// Adds a new account type asynchronously.
    /// </summary>
    /// <param name="accountTypeDomain">The account type to be added.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="Result{AccountTypeDomain}"/> indicating the success or failure of the operation, including the added account type if successful.</returns>
    public Task<Result<AccountTypeDomain>> CreateAccountTypeAsync(AccountTypeDomain accountTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of an existing account type asynchronously.
    /// </summary>
    /// <param name="accountTypeDomain">The account type to update, including its new name and existing identifier.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the update operation.</returns>
    public Task<Result> UpdateAccountTypeName(AccountTypeDomain accountTypeDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds a new currency to the account repository.
    /// </summary>
    /// <param name="currencyDomain">The <see cref="CurrencyDomain"/> object representing the currency to be added.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="Result{CurrencyDomain}"/> object indicating the success or failure of the operation, including the added currency if successful.</returns>
    public Task<Result<CurrencyDomain>> CreateCurrencyAsync(CurrencyDomain currencyDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the currency symbol asynchronously for a specified currency domain object.
    /// </summary>
    /// <param name="currencyDomain">The domain object representing the currency to be updated.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="Result"/> object indicating the success or failure of the update operation.</returns>
    public Task<Result> UpdateCurrencySymbolAsync(CurrencyDomain currencyDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specified currency asynchronously.
    /// </summary>
    /// <param name="currencyDomain">The currency domain object representing the currency to be deleted.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="DeletionResult"/> containing details of the deletion operation, including any related dependencies that were removed.</returns>
    public Task<DeletionResult> DeleteCurrencyAsync(CurrencyDomain currencyDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an account asynchronously.
    /// </summary>
    /// <param name="accountDomain">The account domain object representing the account to be deleted.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="DeletionResult"/> object indicating the status and details of the deletion operation.</returns>
    public Task<DeletionResult> DeleteAccountAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing account asynchronously with the provided account details.
    /// </summary>
    /// <param name="accountDomain">The domain object representing the account to be updated.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="Result"/> object indicating the success or failure of the operation, along with error details, if applicable.</returns>
    public Task<Result> UpdateAccountAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new account asynchronously.
    /// </summary>
    /// <param name="accountDomain">The domain model representing the account to be created.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="Result{AccountDomain}"/> containing the created account details or an error if the operation fails.</returns>
    public Task<Result<AccountDomain>> CreateAccount(AccountDomain accountDomain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total amounts for a specific account asynchronously.
    /// </summary>
    /// <param name="accountDomain">The account domain object representing the account for which the totals are being calculated.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="TotalByAccountDomain"/> object containing the total amounts and related information for the specified account.</returns>
    public Task<TotalByAccountDomain?> GetTotalByAccountAsync(AccountDomain accountDomain, CancellationToken cancellationToken = default);
}