using Domain.Models.Validation;
using MyExpenses.Application.Dtos.Categories;

namespace MyExpenses.Application.Interfaces.IServices;

public interface IExpenseService
{
    /// <summary>
    /// Retrieves all category types available in the system.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="CategoryTypeDto"/> objects.</returns>
    public Task<IEnumerable<CategoryTypeDto>> GetAllCategoryTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new category type to the system.
    /// </summary>
    /// <param name="categoryTypeDto">The data transfer object representing the category type to be added, including its name, color, and other relevant details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> object indicating the success or failure of the operation.</returns>
    public Task<Result> AddCategoryTypeAsync(CategoryTypeDto categoryTypeDto, CancellationToken cancellationToken = default);
}