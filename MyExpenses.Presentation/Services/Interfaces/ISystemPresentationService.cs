using Domain.Models.Dependencies;
using Domain.Models.Validation;
using MyExpenses.Presentation.ViewModels.Accounts;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface ISystemPresentationService
{
    /// <summary>
    /// Retrieves all deletion dependencies for the specified account type.
    /// </summary>
    /// <param name="accountTypeViewModel">The account type view model for which dependencies are to be retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A collection of <see cref="DeletionDependency"/> representing the dependencies associated with the specified account type.</return>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountTypeViewModel accountTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all dependencies for a given category asynchronously.
    /// </summary>
    /// <param name="categoryTypeViewModel">The view model representing the category for which dependencies are being retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains an enumerable collection of <see cref="DeletionDependency"/> objects.</return>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a random color view model asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a <see cref="Result"/>
    /// indicating the operation's success or failure.</return>
    public Task<ColorViewModel> GetRandomColorViewModel(CancellationToken cancellationToken = default);
}