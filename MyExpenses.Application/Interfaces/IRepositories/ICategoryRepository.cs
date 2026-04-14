using Domain.Models.Categories;
using Domain.Models.Validation;

namespace MyExpenses.Application.Interfaces.IRepositories;

public interface ICategoryRepository
{
    /// <summary>
    /// Retrieves all category types from the data source.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of category types.</returns>
    public Task<IEnumerable<CategoryTypeDomain>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new category type asynchronously.
    /// </summary>
    /// <param name="categoryTypeDomain">The category type domain object containing details of the category to add.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A <see cref="Result"/> object indicating the success or failure of the operation.</returns>
    public Task<Result> AddCategoryTypeAsync(CategoryTypeDomain categoryTypeDomain, CancellationToken cancellationToken = default);
}