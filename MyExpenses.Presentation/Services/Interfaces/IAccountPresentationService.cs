using Domain.Models.Validation;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IAccountPresentationService
{
    /// <summary>
    /// Retrieves all account view models asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of <see cref="AccountViewModel"/>.</returns>
    public Task<Result<IEnumerable<AccountViewModel>>> GetAllAccountViewModelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all total by account view models asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a collection of <see cref="TotalByAccountViewModel"/>.</returns>
    public Task<Result<IEnumerable<TotalByAccountViewModel>>> GetAllTotalByAccountViewModelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all account type view models asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a collection of <see cref="AccountTypeViewModel"/>.</return>
    public Task<IEnumerable<AccountTypeViewModel>> GetAllAccountTypeViewModelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all currency view models asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a collection of <see cref="CurrencyViewModel"/>.</return>
    public Task<IEnumerable<CurrencyViewModel>> GetAllCurrencyViewModelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified account type asynchronously.
    /// </summary>
    /// <param name="accountTypeViewModel">The account type view model to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="DeletionResult"/> indicating the success or failure of the deletion.</return>
    public Task<DeletionResult> DeleteAccountTypeAsync(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new account type.
    /// </summary>
    /// <param name="accountTypeViewModel">The view model representing the account type to be added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result{AccountTypeViewModel}"/> indicating the outcome of the operation.</return>
    public Task<Result<AccountTypeViewModel>> AddAccountType(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of an account type.
    /// </summary>
    /// <param name="accountTypeViewModel">The account type model containing the updated name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the update operation.</return>
    public Task<Result> UpdateAccountTypeName(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new currency to the system asynchronously.
    /// </summary>
    /// <param name="newCurrency">The currency details to be added, encapsulated in a <see cref="CurrencyViewModel"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result{CurrencyViewModel}"/> indicating the outcome of the operation.</return>
    public Task<Result<CurrencyViewModel>> AddCurrency(CurrencyViewModel newCurrency, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the currency symbol for the specified currency view model.
    /// </summary>
    /// <param name="currencyViewModel">The view model containing the currency information to be updated.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the operation.</return>
    public Task<Result> UpdateCurrencySymbol(CurrencyViewModel currencyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specified currency asynchronously.
    /// </summary>
    /// <param name="currencyViewModel">The currency to be deleted, represented as a <see cref="CurrencyViewModel"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="DeletionResult"/> indicating the deletion outcome.</return>
    public Task<DeletionResult> DeleteCurrencyAsync(CurrencyViewModel currencyViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an account view model based on the provided total by account view model.
    /// </summary>
    /// <param name="totalByAccountViewModel">The total by account view model.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a single <see cref="AccountViewModel"/>.</returns>
    public Task<Result<AccountViewModel>> GetAccount(TotalByAccountViewModel totalByAccountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified account asynchronously.
    /// </summary>
    /// <param name="accountViewModel">The account to be deleted, represented by an <see cref="AccountViewModel"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a <see cref="DeletionResult"/> indicating the success or failure of the operation.</returns>
    public Task<DeletionResult> DeleteAccountAsync(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing account with the provided information.
    /// </summary>
    /// <param name="accountViewModel">The account view model containing the updated information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the operation.</return>
    public Task<Result> UpdateAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new account asynchronously.
    /// </summary>
    /// <param name="accountViewModel">The account view model containing the details of the account to be created.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result{AccountViewModel}"/> indicating the success or failure of the operation, along with the created account details if successful.</return>
    public Task<Result<AccountViewModel>> CreateAccount(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the total information associated with a specific account view model asynchronously.
    /// </summary>
    /// <param name="accountViewModel">The account view model for which the total information is to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains the <see cref="TotalByAccountViewModel"/> corresponding to the provided account view model.</return>
    public Task<TotalByAccountViewModel?> GetTotalByAccountViewModelAsync(AccountViewModel accountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Merges the data from the source account view model into the destination account view model.
    /// </summary>
    /// <param name="src">The source account view model containing the data to be merged.</param>
    /// <param name="dst">The destination account view model where the data will be merged.</param>
    public void Merge(AccountViewModel src, AccountViewModel dst);
}