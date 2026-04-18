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
}