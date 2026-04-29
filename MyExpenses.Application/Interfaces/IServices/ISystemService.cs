using Domain.Models.Dependencies;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Expenses;

namespace MyExpenses.Application.Interfaces.IServices;

public interface ISystemService
{
    /// <summary>
    /// Retrieves a collection of dependencies associated with the specified account type. Each dependency includes details such as label, count, and category.
    /// </summary>
    /// <param name="accountTypeDto">The account type data transfer object used to identify dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="DeletionDependency"/> objects.</returns>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountTypeDto accountTypeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of dependencies associated with the specified category type. Each dependency includes details such as count and category.
    /// </summary>
    /// <param name="categoryTypeDto">The category type data transfer object used to identify dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="DeletionDependency"/> objects.</returns>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CategoryTypeDto categoryTypeDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a collection of dependencies associated with the specified currency type. Each dependency includes details such as count and category.
    /// </summary>
    /// <param name="currencyDto">The currency type data transfer object used to identify dependencies.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="DeletionDependency"/> objects.</returns>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(CurrencyDto currencyDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all deletion dependencies for the given account.
    /// </summary>
    /// <param name="accountDto">The account data transfer object for which dependencies are to be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of deletion dependencies associated with the account.</returns>
    public Task<IEnumerable<DeletionDependency>> GetAllDependenciesAsync(AccountDto accountDto, CancellationToken cancellationToken = default);
}