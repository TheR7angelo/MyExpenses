using Domain.Models.Validation;
using MyExpenses.Presentation.ViewModels.Accounts;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IAccountPresentationService
{
    /// <summary>
    /// Retrieves all account view models asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a collection of <see cref="AccountViewModel"/>.</return>
    public Task<IEnumerable<AccountViewModel>> GetAllAccountViewModelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all total by account view models asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a collection of <see cref="TotalByAccountViewModel"/>.</return>
    public Task<IEnumerable<TotalByAccountViewModel>> GetAllTotalByAccountViewModelAsync(
        CancellationToken cancellationToken = default);

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
    public Task<DeletionResult> DeleteAccountTypeAsync(AccountTypeViewModel accountTypeViewModel,
        CancellationToken cancellationToken = default);

    // public Task<AccountViewModel> AddOrEditAsync(AccountTypeViewModel accountViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new account type asynchronously.
    /// </summary>
    /// <param name="accountTypeViewModel">The account type to add, represented as an <see cref="AccountTypeViewModel"/>.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the operation.</return>
    public Task<Result> AddAccountType(AccountTypeViewModel accountTypeViewModel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of an account type.
    /// </summary>
    /// <param name="accountTypeViewModel">The account type model containing the updated name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the update operation.</return>
    public Task<Result> UpdateAccountTypeName(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new currency to the system.
    /// </summary>
    /// <param name="newCurrency">The <see cref="CurrencyViewModel"/> representing the currency to be added.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the operation.</return>
    public Task<Result> AddCurrency(CurrencyViewModel newCurrency, CancellationToken cancellationToken = default);

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
    /// Retrieves an account view model associated with the specified total by account view model.
    /// </summary>
    /// <param name="totalByAccountViewModel">An instance of <see cref="TotalByAccountViewModel"/> representing the account details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains an <see cref="AccountViewModel"/>.</return>
    public Task<AccountViewModel?> GetAccount(TotalByAccountViewModel totalByAccountViewModel, CancellationToken cancellationToken = default);

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
}