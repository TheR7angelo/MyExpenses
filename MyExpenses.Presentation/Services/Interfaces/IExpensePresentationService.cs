using Domain.Models.Validation;
using MyExpenses.Presentation.ViewModels.Expenses;

namespace MyExpenses.Presentation.Services.Interfaces;

public interface IExpensePresentationService
{
    /// <summary>
    /// Retrieves all category type view models.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains a collection of category type view models.</return>
    public Task<IEnumerable<CategoryTypeViewModel>> GetAllCategoryTypeViewModelAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new category type.
    /// </summary>
    /// <param name="newCategoryType">The category type to be added, containing its details such as name and color.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains the outcome of the addition operation.</return>
    public Task<Result> AddCategoryType(CategoryTypeViewModel newCategoryType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the provided category type name is available for use.
    /// </summary>
    /// <param name="input">The category type name to check for availability.</param>
    /// <param name="cancellationToken">A cancellation token to observe while awaiting the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a boolean indicating whether the category type name is available.</returns>
    public Task<bool> IsCategoryTypeNameAvailableAsync(string input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specified category type asynchronously.
    /// </summary>
    /// <param name="categoryTypeViewModel">The category type view model to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <return>A task representing the asynchronous operation. The task result contains the outcome of the deletion process.</return>
    public Task<DeletionResult> DeleteCategoryTypeAsync(CategoryTypeViewModel categoryTypeViewModel, CancellationToken cancellationToken = default);
}