using Domain.Models.Dependencies;
using MyExpenses.Application.Dtos.Accounts;
using MyExpenses.Application.Dtos.Categories;

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
}