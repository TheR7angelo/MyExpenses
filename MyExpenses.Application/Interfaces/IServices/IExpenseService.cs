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

    /// <summary>
    /// Deletes a specified category type from the system.
    /// </summary>
    /// <param name="categoryTypeDto">An object representing the category type to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DeletionResult"/> indicating the outcome of the operation, including any deleted dependencies.</returns>
    public Task<DeletionResult> DeleteCategoryTypeAsync(CategoryTypeDto categoryTypeDto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of an existing category type in the system.
    /// </summary>
    /// <param name="categoryTypeDto">An object containing the updated details of the category type, including the new name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public Task<Result> UpdateCategoryTypeNameAsync(CategoryTypeDto categoryTypeDto,
        CancellationToken cancellationToken = default);
}