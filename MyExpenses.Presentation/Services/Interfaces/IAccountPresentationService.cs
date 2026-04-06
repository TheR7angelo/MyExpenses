using Domain.Models.Dependencies;
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
    /// Retrieves all deletion dependencies for the specified account type.
    /// </summary>
    /// <param name="accountTypeViewModel">The account type view model for which dependencies are to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A collection of <see cref="DeletionDependency"/> representing the dependencies associated with the specified account type.</return>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountTypeViewModel accountTypeViewModel,
        CancellationToken cancellationToken = default);
}